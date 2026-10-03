using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using CustomItems.Core;
using CustomItems.Core.Interfaces;
using CustomPlayerEffects;
using CustomItems.Utils;
using UnityEngine;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500T : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        public override uint Id { get; set; } = 7;

        public override string Name { get; set; } = "SCP-500-T";

        public override string Description { get; set; } =
            "<color=#FBFF00>T</color><color=#FAFB00>e</color> <color=#00EEFF>t</color><color=#00E5FB>e</color><color=#00DCF8>l</color><color=#00D3F4>e</color><color=#00CAF1>p</color><color=#00C1ED>o</color><color=#00B8EA>r</color><color=#00AFE6>t</color><color=#00A6E3>a</color> <color=#F7F100>p</color><color=#F7EE00>r</color><color=#F6EA00>a</color> <color=#F4E300>u</color><color=#F4E000>m</color> <color=#FF0000>l</color><color=#DB0000>u</color><color=#B70000>g</color><color=#940000>a</color><color=#700000>r</color> <color=#F2D800>a</color><color=#F1D500>l</color><color=#F0D100>e</color><color=#F0CE00>a</color><color=#EFCB00>t</color><color=#EEC800>ó</color><color=#EDC400>r</color><color=#EDC100>i</color><color=#ECBE00>o</color><color=#EBBB00>.</color>\r\n";
        
        public override float Weight { get; set; } = 1f;

        public string AntesDeTeleportar { get; set; } = "<color=#00EEFF>V</color><color=#00DDF8>o</color><color=#00CDF2>c</color><color=#00BDEC>ê</color><color=#00ADE6>:</color>  <color=#FAFC00>M</color><color=#F9F900>e</color> <color=#F8F300>s</color><color=#F7F000>i</color><color=#F7ED00>n</color><color=#F6EB00>t</color><color=#F5E800>o</color> <color=#F4E200>m</color><color=#F3DF00>e</color><color=#F3DC00>i</color><color=#F2DA00>o</color> <color=#F1D400>e</color><color=#F0D100>n</color><color=#F0CE00>j</color><color=#EFCB00>o</color><color=#EEC900>a</color><color=#EEC600>d</color><color=#EDC300>o</color><color=#ECC000>.</color><color=#ECBD00>.</color><color=#EBBA00>.</color>\r\n";
        public string Teleportando { get; set; } = "<color=#FBFF00>V</color><color=#FAFB00>o</color><color=#F9F700>c</color><color=#F8F400>ê</color> <color=#F6ED00>f</color><color=#F6E900>o</color><color=#F5E600>i</color> <color=#F3DF00>t</color><color=#F2DB00>e</color><color=#F2D700>l</color><color=#F1D400>e</color><color=#F0D000>p</color><color=#EFCD00>o</color><color=#EEC900>r</color><color=#EEC600>t</color><color=#EDC200>a</color><color=#ECBF00>d</color><color=#EBBB00>o</color>\r\n";
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-T</b></color></size>\n" +
            "<size=18><color=#FBFF00>T</color><color=#FAFB00>e</color> <color=#00EEFF>t</color><color=#00E5FB>e</color><color=#00DCF8>l</color><color=#00D3F4>e</color><color=#00CAF1>p</color><color=#00C1ED>o</color><color=#00B8EA>r</color><color=#00AFE6>t</color><color=#00A6E3>a</color> <color=#F7F100>p</color><color=#F7EE00>r</color><color=#F6EA00>a</color> <color=#F4E300>u</color><color=#F4E000>m</color> <color=#FF0000>l</color><color=#DB0000>u</color><color=#B70000>g</color><color=#940000>a</color><color=#700000>r</color> <color=#F2D800>a</color><color=#F1D500>l</color><color=#F0D100>e</color><color=#F0CE00>a</color><color=#EFCB00>t</color><color=#EEC800>ó</color><color=#EDC400>r</color><color=#EDC100>i</color><color=#ECBE00>o</color><color=#EBBB00>.</color></size>";

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
            Exiled.Events.Handlers.Player.CancellingItemUse += OnCancellingItemUse;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.CancellingItemUse -= OnCancellingItemUse;
            base.UnsubscribeEvents();
        }

        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;  // Impede parar o uso do item
        }

        //Teleports player to random room after using item
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;

            Timing.CallDelayed(0.75f, () =>
            {
                ev.Item.Destroy(); //Destroys item after using
                Execute(ev);
            });
        }

        private void Execute(UsingItemEventArgs ev)
        {
            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint("Você é um SCP, portanto não sofreu mudanças do uso deste item.");
                return;
            }

            //The best more beautiful method I found to get random room
            Room randomRoom = GetRoom();   

            //Saves all active effects before altering them
            List<Effect> effectsToRestore = [.. ev.Player.ReferenceHub.playerEffectsController.AllEffects.Select(e => new Effect(e))];
            byte slownessIntensity = ev.Player.GetEffect<Slowness>().Intensity; //Get slowness intensity to apply tween

            //I thought it would be more balanced for it to have a "Cutscene" so that he doesn't just teleports instantly yk
            ev.Player.ShowHint(AntesDeTeleportar);
            
            Tweens.TweenEffect(ev.Player, EffectType.Blurred, 0, 1, 0, 1f, Ease.InBounce);
            Tweens.TweenEffect(ev.Player, EffectType.Slowness, slownessIntensity, 100, 0, 3f, Ease.InBounce);

            Timing.CallDelayed(3f, () =>
            {
                ev.Player.ChangeEffectIntensity(EffectType.Ensnared, 255, 10f);
                ev.Player.ChangeEffectIntensity(EffectType.Flashed, 255, 10f);
                Tweens.TweenEffect(ev.Player, EffectType.Fade, 0, 255, 0, 2f);
            });

            Timing.CallDelayed(5f, () =>
            {
                Tweens.TweenEffect(ev.Player, EffectType.Fade, 255, 0, 0, 3f);
                
                //Restore all effects
                effectsToRestore.ForEach((effect) =>
                    ev.Player.ChangeEffectIntensity(effect.Type, effect.Intensity, effect.Duration));

                HandleTeleport(ev.Player, randomRoom);
                ev.Player.ShowHint(Teleportando);
                Log.Debug(randomRoom.ToString());
            });
        }

        private void HandleTeleport(Player player, Room room)
        {
            if (room.Type == RoomType.Pocket)
                player.ChangeEffectIntensity(EffectType.PocketCorroding, 1, 0);
            else
                player.Teleport(room);
        }

        private static Room GetRoom()
        {
            if (Warhead.IsDetonated)
            {
                return Room.Get(room => room.Zone == ZoneType.Surface).GetRandomValue();
            }

            if (Map.IsLczDecontaminated)
            {
                return Room.Get(room => IsValidRoom(room) && room.Zone != ZoneType.LightContainment).GetRandomValue();
            }
            
            return Room.Get(IsValidRoom).GetRandomValue();
        }

        private static bool IsValidRoom(Room room)
        {
            return room.Type != RoomType.EzCollapsedTunnel &&
                   room.Type != RoomType.EzShelter &&
                   room.Type != RoomType.Unknown &&
                   room.Type != RoomType.Surface;
        }
    }
}
