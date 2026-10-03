using System.Linq;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using PlayerRoles;
using Scp035.Managers;
using VoiceChat;

namespace Scp035
{
    public sealed class EventHandlers
    {
        private readonly Plugin plugin;
        private readonly Scp035Manager manager;

        internal EventHandlers(Plugin plugin, Scp035Manager manager)
        {
            this.plugin = plugin;
            this.manager = manager;
        }

        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Player == null || ev.Pickup == null || !manager.Is035(ev.Player))
                return;

            // Bloqueio mais cedo possível: o 035 nem inicia a barra/animação de pegar.
            if (manager.IsForbiddenVanillaItemType(ev.Pickup.Type))
            {
                ev.IsAllowed = false;
                string name = manager.GetForbiddenVanillaDisplayName(ev.Pickup.Type);
                ev.Player.ShowHint($"<color=#ff3030>O SCP-035 não pode interagir com {name}.</color>", 2.5f);
                return;
            }

            if (manager.IsCachedForbiddenPickup(ev.Pickup, out string forbiddenName))
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint($"<color=#ff3030>O SCP-035 não pode interagir com {forbiddenName}.</color>", 2.5f);
            }
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || ev.Pickup == null || !manager.Is035(ev.Player))
                return;

            if (manager.IsForbiddenVanillaItemType(ev.Pickup.Type))
            {
                ev.IsAllowed = false;
                string name = manager.GetForbiddenVanillaDisplayName(ev.Pickup.Type);
                ev.Player.ShowHint($"<color=#ff3030>O SCP-035 não pode pegar {name}.</color>", 2.5f);
                return;
            }

            // Cache preguiçoso: verifica apenas ESTE pickup quando o 035 tenta interagir.
            // Não existe mais scanner global de Pickup.List.
            if (manager.IsCachedForbiddenPickup(ev.Pickup, out string forbiddenName))
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint($"<color=#ff3030>O SCP-035 não pode interagir com {forbiddenName}.</color>", 2.5f);
                return;
            }

            // Última barreira de segurança somente após uma interação real.
            Timing.CallDelayed(0.08f, () =>
            {
                if (manager.Is035(ev.Player))
                    manager.RemoveForbiddenItems(ev.Player);
            });
        }

        public void OnUsingItem(UsingItemEventArgs ev)
        {
            if (ev.Player == null || ev.Item == null)
                return;

            // Não confie apenas no ID: outros plugins podem reutilizar o mesmo CustomItem ID.
            // O item de possessão do 035 precisa ser especificamente o SCP-1344 registrado como ID configurado.
            if (ev.Item.Type != ItemType.SCP1344)
                return;

            if (!CustomItem.TryGet(ev.Item, out CustomItem customItem) || customItem.Id != plugin.Config.Scp035ItemId)
                return;

            if (manager.IsBusy || manager.IsActive)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint("<color=#ff3030>O SCP-035 já está ativo nesta rodada.</color>", 3f);
                return;
            }

            manager.BeginPossession(ev.Player, ev.Item);
        }

        public void OnTransmitting(TransmittingEventArgs ev)
        {
            if (!plugin.Config.HearScpChatButSpeakProximity || ev.Player == null || !manager.Is035(ev.Player))
                return;

            // O controller continua com VoiceChannel = ScpChat para OUVIR o canal SCP.
            // Porém, todo pacote de voz originado do 035 é reescrito para proximidade.
            VoiceChat.Networking.VoiceMessage message = ev.VoiceMessage;
            message.Channel = VoiceChatChannel.Proximity;
            ev.VoiceMessage = message;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (!plugin.Config.BlockAlliedFriendlyFire || ev.Player == null || ev.Attacker == null)
                return;

            bool victimIs035 = manager.Is035(ev.Player);
            bool attackerIs035 = manager.Is035(ev.Attacker);

            if (!victimIs035 && !attackerIs035)
                return;

            // A role REAL do SCP-035 é Tutorial. A aparência humana é apenas visual.
            // Portanto, humanos de QUALQUER facção podem dar dano no 035 e receber dano dele.
            // A única imunidade mútua mantida é entre o 035 e os outros SCPs.
            if (victimIs035 && ev.Attacker.IsScp)
            {
                ev.IsAllowed = false;
                return;
            }

            if (attackerIs035 && ev.Player.IsScp)
                ev.IsAllowed = false;
        }

        public void On096AddingTarget(AddingTargetEventArgs ev)
        {
            if (plugin.Config.IgnoreScp096Trigger && ev.Target != null && manager.Is035(ev.Target))
                ev.IsAllowed = false;
        }

        public void On173AddingObserver(AddingObserverEventArgs ev)
        {
            if (plugin.Config.IgnoreScp173Observation && ev.Observer != null && manager.Is035(ev.Observer))
                ev.IsAllowed = false;
        }

        public void On173BeingObserved(BeingObservedEventArgs ev)
        {
            if (plugin.Config.IgnoreScp173Observation && ev.Target != null && manager.Is035(ev.Target))
                ev.IsAllowed = false;
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (manager.Is035(ev.Player))
                manager.On035Died(ev.Player, ev.Attacker, ev.DamageHandler?.Type ?? Exiled.API.Enums.DamageType.Unknown);
        }

        public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (manager.SuppressRagdoll.Contains(ev.Player))
                ev.IsAllowed = false;
        }

        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!manager.IsActive)
                return;

            int humans = ev.ClassList.scientists + ev.ClassList.class_ds +
                         ev.ClassList.mtf_and_guards + ev.ClassList.chaos_insurgents;
            int scps = ev.ClassList.scps_except_zombies + ev.ClassList.zombies;

            if (humans > 0 && (scps > 0 || manager.IsActive))
                ev.IsAllowed = false;
        }

        public void OnRoundStarted()
        {
            if (plugin.Config.LogNaturalSpawnLocation)
                manager.BeginNaturalSpawnDiagnostics();
        }

        public void OnWaitingForPlayers() => manager.Reset(false);
        public void OnRestartingRound() => manager.Reset(false);
    }
}
