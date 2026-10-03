using CustomItems.Core;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using CustomItems.Core.Interfaces;
using CustomItems.Utils;
using UnityEngine;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500H : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        private System.Random rnd = new System.Random();

        private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public override uint Id { get; set; } = 4;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-500-H";

        private Side oppositeSide { get; set; }

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "<color=#FBFF00>A</color><color=#FAFC00>u</color><color=#F9F900>m</color><color=#F9F600>e</color><color=#F8F300>n</color><color=#F7F000>t</color><color=#F7ED00>a</color> <color=#00EEFF>2</color><color=#00C5EF>0</color> <color=#F5E500>d</color><color=#F4E200>e</color> <color=#51FF00>v</color><color=#3CDF03>i</color><color=#28BF07>d</color><color=#149F0B>a</color> <color=#F2DA00>m</color><color=#F2D700>á</color><color=#F1D400>x</color><color=#F0D100>i</color><color=#F0CE00>m</color><color=#EFCB00>a</color><color=#EEC900>,</color> <color=#EDC300>a</color><color=#ECC000>t</color><color=#ECBD00>é</color> <color=#00EEFF>2</color><color=#00D3F4>0</color><color=#00B8EA>0</color>\r\n";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1f;

        public List<Player> enhancedPlayers = new List<Player>
        {

        };

        /// <inheritdoc/>
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-H</b></color></size>\n" +
            "<size=18><color=#FBFF00>A</color><color=#FAFC00>u</color><color=#F9F900>m</color><color=#F9F600>e</color><color=#F8F300>n</color><color=#F7F000>t</color><color=#F7ED00>a</color> <color=#00EEFF>2</color><color=#00C5EF>0</color> <color=#F5E500>d</color><color=#F4E200>e</color> <color=#51FF00>v</color><color=#3CDF03>i</color><color=#28BF07>d</color><color=#149F0B>a</color> <color=#F2DA00>m</color><color=#F2D700>á</color><color=#F1D400>x</color><color=#F0D100>i</color><color=#F0CE00>m</color><color=#EFCB00>a</color><color=#EEC900>,</color> <color=#EDC300>a</color><color=#ECC000>t</color><color=#ECBD00>é</color> <color=#00EEFF>2</color><color=#00D3F4>0</color><color=#00B8EA>0</color></size>";

        // Remove as mensagens automáticas do Exiled.CustomItems.
        // O CustomHeldHintService cuida da interface visual.
        protected override void ShowPickedUpMessage(Exiled.API.Features.Player player) { }
        protected override void ShowSelectedMessage(Exiled.API.Features.Player player) { }

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            // O spawn natural é controlado pelo ServerHandler.
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(),
            LockerSpawnPoints = new List<LockerSpawnPoint>(),
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = Color.yellow;
        public float GlowRange { get; set; } = 0.2f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } =  ICustomItemGlow.GlowShadowType.Soft;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.Died += OnDeath;
            Exiled.Events.Handlers.Player.CancellingItemUse += OnCancellingItemUse;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Player.Died -= OnDeath;
            Exiled.Events.Handlers.Player.CancellingItemUse -= OnCancellingItemUse;
            base.UnsubscribeEvents();
        }

        // Evento de cancelamento de uso de item
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            if (enhancedPlayers.Contains(ev.Player))
            {
                // Subtrai a saúde do jogador em 20 pontos

                ev.Player.MaxHealth -= 20;

                // remove o jogador à lista de enhancedPlayers

                enhancedPlayers.Remove(ev.Player);
            }
        }

        //Increases player's max health
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            // Verifica se o item não é válido ou se a vida do jogador já é maior ou igual a 200

            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;

            if (ev.Player.MaxHealth >= 200)
            {
                ev.Player.ShowHint("Sua vida máxima já está no limite desta pílula.", 3f);
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            // Aguarda 2 segundos antes de executar a lógica
            Timing.WaitForSeconds(2f);

            // Verifica se o jogador é um SCP (caso sim, não há mudança no jogador)
            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint("Você é um SCP, portanto não sofreu mudanças do uso deste item.");
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            // Aumenta a saúde do jogador em 20 pontos
            ev.Player.MaxHealth += 20;

            // Verifica se o jogador já está na lista de players aprimorados
            if (enhancedPlayers.Contains(ev.Player))
            {
                Timing.CallDelayed(1.3f, ev.Item.Destroy);
                return;
            }

            // Se não, adiciona o jogador à lista de enhancedPlayers
            enhancedPlayers.Add(ev.Player);

            // O uso vanilla foi cancelado para não curar HP; consumimos manualmente.
            Timing.CallDelayed(1.3f, ev.Item.Destroy);
        }

        private void OnEscaping(EscapingEventArgs ev)
         {
             var playerHealth = ev.Player.MaxHealth;
             if (enhancedPlayers.Contains(ev.Player))
             {
                 Timing.CallDelayed(1f, () =>
                 {
                     ev.Player.MaxHealth = playerHealth;
                     ev.Player.Health = playerHealth;
                     ev.Player.ShowHint("Como você tinha " + playerHealth.ToString() + " de vida por ter utilizado o SCP-500-H, sua vida foi restaurada para este valor após escapar.", 30);
                     enhancedPlayers.Remove(ev.Player);
                 });
             }
        }
        private void OnDeath(DiedEventArgs ev)
        {
            if (ev.Player != null && enhancedPlayers.Contains(ev.Player))
            enhancedPlayers.Remove(ev.Player);
        }
    }
}
