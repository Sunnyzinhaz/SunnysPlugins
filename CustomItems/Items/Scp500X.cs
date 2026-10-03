using CustomItems.Core;
using CustomItems.Utils;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Core.Interfaces;
using UnityEngine;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500X : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        private System.Random rnd = new System.Random();

        private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public override uint Id { get; set; } = 8;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-500-X";

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "<color=#FBFF00>A</color><color=#FAFC00>b</color><color=#F9FA00>r</color><color=#F9F800>e</color> <color=#F8F300>a</color><color=#F7F100>s</color> <color=#00EEFF>p</color><color=#00E0F9>o</color><color=#00D3F4>r</color><color=#00C5EF>t</color><color=#00B8EA>a</color><color=#00AAE5>s</color> <color=#F6EA00>m</color><color=#F5E800>a</color><color=#F5E500>i</color><color=#F4E300>s</color> <color=#F3DE00>p</color><color=#F3DC00>r</color><color=#F2DA00>ó</color><color=#F2D800>x</color><color=#F1D500>i</color><color=#F1D300>m</color><color=#F0D100>a</color><color=#F0CE00>s</color> <color=#EFCA00>d</color><color=#EEC800>e</color> <color=#EDC300>v</color><color=#EDC100>o</color><color=#ECBE00>c</color><color=#ECBC00>ê</color><color=#EBBA00>.</color>\n";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1f;

        /// <inheritdoc/>
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-X</b></color></size>\n" +
            "<size=18><color=#FBFF00>A</color><color=#FAFC00>b</color><color=#F9FA00>r</color><color=#F9F800>e</color> <color=#F8F300>a</color><color=#F7F100>s</color> <color=#00EEFF>p</color><color=#00E0F9>o</color><color=#00D3F4>r</color><color=#00C5EF>t</color><color=#00B8EA>a</color><color=#00AAE5>s</color> <color=#F6EA00>m</color><color=#F5E800>a</color><color=#F5E500>i</color><color=#F4E300>s</color> <color=#F3DE00>p</color><color=#F3DC00>r</color><color=#F2DA00>ó</color><color=#F2D800>x</color><color=#F1D500>i</color><color=#F1D300>m</color><color=#F0D100>a</color><color=#F0CE00>s</color> <color=#EFCA00>d</color><color=#EEC800>e</color> <color=#EDC300>v</color><color=#EDC100>o</color><color=#ECBE00>c</color><color=#ECBC00>ê</color><color=#EBBA00>.</color></size>";

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
            // Marca o item como sendo cancelado
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;  // Impede o uso do item
            }
        }
        //Opens doors near the player
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;
            
            Timing.WaitForSeconds(2f);

            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint("Você é um SCP, portanto não sofreu mudanças do uso deste item.");
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            var doors = ev.Player.CurrentRoom.Doors.ToList();
            foreach (Door door in doors)
            {
                if (door.IsGate) continue;
                door.IsOpen = true;
            }
            Timing.CallDelayed(2f, () =>
            {
                // Destrói o item após o uso
                ev.Item.Destroy();
            });
        }
    }
}
