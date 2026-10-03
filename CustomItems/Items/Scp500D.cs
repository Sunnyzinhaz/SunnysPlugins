using CustomItems.Core;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Core.Interfaces;
using CustomItems.Utils;
using UnityEngine;

namespace CustomItems.Items
{

    [CustomItem(ItemType.SCP500)]
    public class Scp500D : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        private System.Random rnd = new System.Random();

        private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public override uint Id { get; set; } = 3;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-500-D";

        private Side oppositeSide { get; set; }

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "<color=#FBFF00>T</color><color=#FAFD00>r</color><color=#FAFC00>a</color><color=#FAFB00>n</color><color=#F9FA00>s</color><color=#F9F800>f</color><color=#F9F700>o</color><color=#F9F600>r</color><color=#F8F500>m</color><color=#F8F300>a</color> <color=#F7F100>s</color><color=#F7F000>u</color><color=#F7EE00>a</color> <color=#F6EC00>a</color><color=#F6EB00>p</color><color=#F6E900>a</color><color=#F5E800>r</color><color=#F5E700>ê</color><color=#F5E600>n</color><color=#F5E400>c</color><color=#F4E300>i</color><color=#F4E200>a</color> <color=#00EEFF>t</color><color=#00E8FC>e</color><color=#00E3FA>m</color><color=#00DDF8>p</color><color=#00D8F6>o</color><color=#00D2F4>r</color><color=#00CDF2>a</color><color=#00C8F0>r</color><color=#00C2EE>i</color><color=#00BDEC>a</color><color=#00B7EA>m</color><color=#00B2E8>e</color><color=#00ADE6>n</color><color=#00A7E4>t</color><color=#00A2E2>e</color> <color=#F3DE00>n</color><color=#F3DD00>a</color> <color=#F2DA00>d</color><color=#F2D900>e</color> <color=#F2D700>u</color><color=#F1D500>m</color> <color=#F1D300>m</color><color=#F0D200>e</color><color=#F0D000>m</color><color=#F0CF00>b</color><color=#F0CE00>r</color><color=#EFCD00>o</color> <color=#EFCA00>d</color><color=#EEC900>o</color> <color=#8D33FA>t</color><color=#7F26F8>i</color><color=#7119F7>m</color><color=#630CF6>e</color> <color=#ECC000>o</color><color=#ECBF00>p</color><color=#ECBE00>o</color><color=#ECBC00>s</color><color=#EBBB00>t</color><color=#EBBA00>o</color><color=#EBB900>.</color>\n";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1f;

        /// <inheritdoc/>
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-D</b></color></size>\n" +
            "<size=18><color=#FBFF00>T</color><color=#FAFD00>r</color><color=#FAFC00>a</color><color=#FAFB00>n</color><color=#F9FA00>s</color><color=#F9F800>f</color><color=#F9F700>o</color><color=#F9F600>r</color><color=#F8F500>m</color><color=#F8F300>a</color> <color=#F7F100>s</color><color=#F7F000>u</color><color=#F7EE00>a</color> <color=#F6EC00>a</color><color=#F6EB00>p</color><color=#F6E900>a</color><color=#F5E800>r</color><color=#F5E700>ê</color><color=#F5E600>n</color><color=#F5E400>c</color><color=#F4E300>i</color><color=#F4E200>a</color> <color=#00EEFF>t</color><color=#00E8FC>e</color><color=#00E3FA>m</color><color=#00DDF8>p</color><color=#00D8F6>o</color><color=#00D2F4>r</color><color=#00CDF2>a</color><color=#00C8F0>r</color><color=#00C2EE>i</color><color=#00BDEC>a</color><color=#00B7EA>m</color><color=#00B2E8>e</color><color=#00ADE6>n</color><color=#00A7E4>t</color><color=#00A2E2>e</color> <color=#F3DE00>n</color><color=#F3DD00>a</color> <color=#F2DA00>d</color><color=#F2D900>e</color> <color=#F2D700>u</color><color=#F1D500>m</color> <color=#F1D300>m</color><color=#F0D200>e</color><color=#F0D000>m</color><color=#F0CF00>b</color><color=#F0CE00>r</color><color=#EFCD00>o</color> <color=#EFCA00>d</color><color=#EEC900>o</color> <color=#8D33FA>t</color><color=#7F26F8>i</color><color=#7119F7>m</color><color=#630CF6>e</color> <color=#ECC000>o</color><color=#ECBF00>p</color><color=#ECBE00>o</color><color=#ECBC00>s</color><color=#EBBB00>t</color><color=#EBBA00>o</color><color=#EBB900>.</color></size>";

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

        public List<RoleTypeId> foundationRoles = new List<RoleTypeId>()
        {
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain,
            RoleTypeId.NtfSpecialist,
            RoleTypeId.Scientist
        };

        public List<RoleTypeId> foundationEnemyRoles = new List<RoleTypeId>()
        {
            RoleTypeId.ChaosMarauder,
            RoleTypeId.ChaosConscript,
            RoleTypeId.ChaosRepressor,
            RoleTypeId.ChaosRifleman,
            RoleTypeId.ClassD,
            RoleTypeId.Tutorial
        };
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            // Marca o item como sendo cancelado
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;  // Impede o uso do item
            }
        }
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
            var oldRole = ev.Player.Role.Type;

            if (foundationEnemyRoles.Contains(ev.Player.Role.Type))
                Timing.CallDelayed(1f, () =>
                {
                    var selectedRole = foundationRoles.RandomItem();
                    ev.Player.ChangeAppearance(selectedRole, true);
                    ev.Player.ShowHint("Você mudou a sua aparência para um " + Convert.ToString(selectedRole));
                    Log.Debug("New role: " + Convert.ToString(selectedRole) + " Old role: " + Convert.ToString(oldRole));
                });

            else if (foundationRoles.Contains(ev.Player.Role.Type))
                Timing.CallDelayed(1f, () =>
                {
                    var selectedRole = foundationEnemyRoles.RandomItem();
                    ev.Player.ChangeAppearance(selectedRole, true);
                    ev.Player.ShowHint("Você mudou a sua aparência para um " + Convert.ToString(selectedRole));
                    Log.Debug("New role: " + Convert.ToString(selectedRole) + " Old role: " + Convert.ToString(oldRole));
                });

            Timing.CallDelayed(31f, () =>
            {
                ev.Player.ChangeAppearance(oldRole, true);
                ev.Player.ShowHint("Sua aparência voltou ao normal");
            });
            Timing.CallDelayed(1f, () =>
            {
                // Destrói o item após o uso
                ev.Item.Destroy();
            });
        }
    }
}
