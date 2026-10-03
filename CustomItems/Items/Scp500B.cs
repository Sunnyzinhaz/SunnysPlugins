using CustomItems.Core;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Core.Interfaces;
using MEC;
using CustomItems.Utils;
using Exiled.API.Extensions;
using Exiled.API.Features;
using UnityEngine;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500B : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        public override uint Id { get; set; } = 2;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-500-B";

        private Side oppositeSide { get; set; }

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "<color=#FBFF00>E</color><color=#FBFD00>s</color><color=#FBFC00>t</color><color=#FBFB00>a</color> <color=#FBF800>p</color><color=#FBF700>i</color><color=#FBF600>l</color><color=#FBF500>u</color><color=#FBF300>l</color><color=#FBF200>a</color> <color=#FCF000>f</color><color=#FCEF00>a</color><color=#FCED00>z</color> <color=#FCEB00>v</color><color=#FCEA00>o</color><color=#FCE800>c</color><color=#FCE700>ê</color> <color=#00EEFF>t</color><color=#00E0F9>r</color><color=#00D3F4>o</color><color=#00C5EF>c</color><color=#00B8EA>a</color><color=#00AAE5>r</color> <color=#FDE200>d</color><color=#FDE100>e</color> <color=#8000FF>t</color><color=#6B00E8>i</color><color=#5600D1>m</color><color=#4100BA>e</color> <color=#FDDC00>a</color><color=#FDDB00>o</color> <color=#FDD900>s</color><color=#FDD700>e</color><color=#FDD600>r</color> <color=#FED400>c</color><color=#FED200>o</color><color=#FED100>n</color><color=#FED000>s</color><color=#FECF00>u</color><color=#FECE00>m</color><color=#FECC00>i</color><color=#FECB00>d</color><color=#FECA00>a</color><color=#FEC900>.</color>\n";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1f;

        /// <inheritdoc/>
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-B</b></color></size>\n" +
            "<size=18><color=#FBFF00>E</color><color=#FBFD00>s</color><color=#FBFC00>t</color><color=#FBFB00>a</color> <color=#FBF800>p</color><color=#FBF700>i</color><color=#FBF600>l</color><color=#FBF500>u</color><color=#FBF300>l</color><color=#FBF200>a</color> <color=#FCF000>f</color><color=#FCEF00>a</color><color=#FCED00>z</color> <color=#FCEB00>v</color><color=#FCEA00>o</color><color=#FCE800>c</color><color=#FCE700>ê</color> <color=#00EEFF>t</color><color=#00E0F9>r</color><color=#00D3F4>o</color><color=#00C5EF>c</color><color=#00B8EA>a</color><color=#00AAE5>r</color> <color=#FDE200>d</color><color=#FDE100>e</color> <color=#8000FF>t</color><color=#6B00E8>i</color><color=#5600D1>m</color><color=#4100BA>e</color> <color=#FDDC00>a</color><color=#FDDB00>o</color> <color=#FDD900>s</color><color=#FDD700>e</color><color=#FDD600>r</color> <color=#FED400>c</color><color=#FED200>o</color><color=#FED100>n</color><color=#FED000>s</color><color=#FECF00>u</color><color=#FECE00>m</color><color=#FECC00>i</color><color=#FECB00>d</color><color=#FECA00>a</color><color=#FEC900>.</color> </size>";

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

        //Only method that I found for selecting a random mtf/chaos role was making a list with those role types and choosing a random value as you will see in the next lines of code
        public List<RoleTypeId> mtfRoles =
        [
            //RoleTypeId.NtfSpecialist,
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain,
        ];

        public List<RoleTypeId> chaosRoles =
        [
            //RoleTypeId.ChaosMarauder,
            RoleTypeId.ChaosConscript,
            RoleTypeId.ChaosRepressor,
            RoleTypeId.ChaosRifleman,
        ];
        
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            
            // Impede o cancelamento do uso de item
            ev.IsAllowed = false;
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;

            Timing.CallDelayed(1.2f, () =>
            {
                ev.Item.Destroy();
                Execute(ev);
            });
        }

        //Changes player's team on using item
        private void Execute(UsingItemEventArgs ev)
        {
            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint("Você é um SCP, portanto não sofreu mudanças do uso deste item.");
                return;
            }

            RoleTypeId randomRole = GetRandomRole(ev.Player.Role.Type);
            var oldEffects = ev.Player.ActiveEffects.ToList().Select(e => new Effect(e));
            ev.Player.Role.Set(randomRole, RoleSpawnFlags.None);
            ev.Player.SyncEffects(oldEffects);
        }

        private RoleTypeId GetRandomRole(RoleTypeId role)
        {
            Side side = role.GetSide();

            if (role == RoleTypeId.None || role == RoleTypeId.Tutorial || side == Side.Scp)
                return role;

            if (side == Side.Mtf && role != RoleTypeId.Scientist)
                return chaosRoles.RandomItem();
            else if (side == Side.ChaosInsurgency && role != RoleTypeId.ClassD)
                return mtfRoles.RandomItem();

            return role == RoleTypeId.ClassD ? RoleTypeId.Scientist : RoleTypeId.ClassD;
        }
    }
}
