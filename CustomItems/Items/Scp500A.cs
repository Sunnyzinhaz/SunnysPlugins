using CustomItems.Core;
using CustomItems.Utils;
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
using UnityEngine;
using Random = System.Random;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500A : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        private Random rnd = new();

        private List<CoroutineHandle> coroutines = new();

        public override uint Id { get; set; } = 1;

        public override string Name { get; set; } = "SCP-500-A";
        public override string Description { get; set; } =
            "<color=#FBFF00>F</color><color=#FBFC00>a</color><color=#FBF900>z</color> <color=#FBF300>u</color><color=#FCF000>m</color> <color=#00EEFF>a</color><color=#00E0F9>l</color><color=#00D3F4>i</color><color=#00C5EF>a</color><color=#00B8EA>d</color><color=#00AAE5>o</color> <color=#11FF00>v</color><color=#0EE300>o</color><color=#0CC800>l</color><color=#09AD00>t</color><color=#079200>a</color><color=#047700>r</color> <color=#FCE700>d</color><color=#FCE400>o</color><color=#FDE200>s</color> <color=#FDDC00>m</color><color=#FDD900>o</color><color=#FDD600>r</color><color=#FED300>t</color><color=#FED000>o</color><color=#FECD00>s</color><color=#FECA00>.</color>\n";

        public override float Weight { get; set; } = 1f;

        /// <inheritdoc/>
        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-A</b></color></size>\n" +
            "<size=18><color=#FBFF00>F</color><color=#FBFC00>a</color><color=#FBF900>z</color> <color=#FBF300>u</color><color=#FCF000>m</color> <color=#00EEFF>a</color><color=#00E0F9>l</color><color=#00D3F4>i</color><color=#00C5EF>a</color><color=#00B8EA>d</color><color=#00AAE5>o</color> <color=#11FF00>v</color><color=#0EE300>o</color><color=#0CC800>l</color><color=#09AD00>t</color><color=#079200>a</color><color=#047700>r</color> <color=#FCE700>d</color><color=#FCE400>o</color><color=#FDE200>s</color> <color=#FDDC00>m</color><color=#FDD900>o</color><color=#FDD600>r</color><color=#FED300>t</color><color=#FED000>o</color><color=#FECD00>s</color><color=#FECA00>.</color></size>";

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

        public List<RoleTypeId> mtfRoles = new List<RoleTypeId>()
        {
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain,
            RoleTypeId.NtfSpecialist
        };

        public List<RoleTypeId> chaosRoles = new List<RoleTypeId>()
        {
            RoleTypeId.ChaosMarauder,
            RoleTypeId.ChaosConscript,
            RoleTypeId.ChaosRepressor,
            RoleTypeId.ChaosRifleman,
        };

        //Spawns spectator on using item
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            // Marca o item como sendo cancelado
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;  // Impede o cancelamento do item
            }
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;

            //!System sleep
            //!System.Threading.Thread.Sleep(2000);

            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint("Você é um SCP, portanto não sofreu mudanças do uso deste item.");
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            if (ev.Player.CurrentRoom.Type is RoomType.Pocket or RoomType.Unknown)
            {
                ev.Player.ShowHint("Você não pode usar esse item aqui!");
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            var deadPlayers = Player.List.Where(p => p.IsDead && p.Role.Type != RoleTypeId.Overwatch);
            var randomSpectator = deadPlayers.GetRandomValue();

            if (randomSpectator == null)
            {
                ev.Player.ShowHint("Você tentou puxar alguém de seu descanso... mas não há ninguém para ressuscitar.", 5f);
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }

            // Found that it would be good to teleport respawned player to the player that Using the pill
            var playerPos = ev.Player.Position;

            RoleTypeId randomRole = RoleTypeId.Tutorial;
            if (ev.Player.Role.Type != RoleTypeId.Tutorial)
                randomRole = ev.Player.Role.Side == Side.Mtf ? mtfRoles.RandomItem() : chaosRoles.RandomItem();

            randomSpectator.Role.Set(randomRole);
            randomSpectator.ShowHint("Você foi ressuscitado por " + Convert.ToString(ev.Player.Nickname) + " e sua nova classe é " + Convert.ToString(randomRole), 5f);
            randomSpectator.Position = playerPos;
            
            // Destrói o item após o uso
            // Chama a destruição do item após um pequeno delay
            Timing.CallDelayed(1.5f, ev.Item.Destroy);
        }
    }
}
