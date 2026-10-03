using System.Collections.Generic;
using System.Linq;
using CustomItems.Core.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

using Light = LabApi.Features.Wrappers.AdminToy;
using Map = Exiled.Events.Handlers.Map;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Server = Exiled.Events.Handlers.Server;

// This code is derived from SnivysUltimatePackage
// Copyright © SnivyFilms
// Licensed under the GNU Affero General Public License v3.0
//
// Original source:
// https://github.com/SnivyFilms/SnivysUltimatePackage
namespace CustomItems.Events
{
    public class GlowEventsHandler
    {
        private static readonly Dictionary<Pickup, Light> ActiveGlowEffects = new Dictionary<Pickup, Light>();

        public GlowEventsHandler()
        {
            Server.RoundStarted += OnRoundStarted;
            Server.WaitingForPlayers += OnWaitingForPlayers;
            Map.PickupAdded += AddGlow;
            Map.PickupDestroyed += RemoveGlow;
        }
        
        ~GlowEventsHandler()
        {
            Server.RoundStarted -= OnRoundStarted;
            Server.WaitingForPlayers -= OnWaitingForPlayers;
            Map.PickupAdded -= AddGlow;
            Map.PickupDestroyed -= RemoveGlow;
        }

        public void AddGlow(PickupAddedEventArgs ev) => HandleGlowEffect(ev.Pickup);

        public void RemoveGlow(PickupDestroyedEventArgs ev)
        {
            if (ev.Pickup == null || ev.Pickup?.Base?.gameObject == null)
                return;

            if (ActiveGlowEffects.ContainsKey(ev.Pickup))
                RemoveGlowEffect(ev.Pickup);
        }
        
        private void OnRoundStarted()
        {
            Timing.CallDelayed(1, () =>
            {
                foreach (Pickup pickup in Pickup.List)
                    HandleGlowEffect(pickup);
            });
        }

        private void OnWaitingForPlayers() => ClearAllGlowEffects();

        private void HandleGlowEffect(Pickup pickup)
        {
            if (pickup == null)
                return;

            if (!CustomItem.TryGet(pickup, out CustomItem ci))
                return;
            if (ci is not ICustomItemGlow { HasCustomItemGlow: true } glowableItem)
                return;

            ApplyGlowEffect(pickup, glowableItem.CustomItemGlowColor, glowableItem.GlowRange, glowableItem.GlowIntensity, glowableItem.ShadowType, glowableItem.GlowOffset);
        }

        private void ApplyGlowEffect(Pickup pickup, Color glowColor, float range = 0.25f, float intensity = 1f, ICustomItemGlow.GlowShadowType shadowType = ICustomItemGlow.GlowShadowType.None, Vector3? offset = null)
        {
            if (ActiveGlowEffects.ContainsKey(pickup))
            {
                RemoveGlowEffect(pickup);
            }
            var actualOffset = offset ?? Vector3.zero;
            var light = LightSourceToy.Create(pickup.Position, networkSpawn: false);
            light.Color = glowColor;
            light.Intensity = intensity;
            light.Range = range;
            light.ShadowType = (LightShadows)shadowType;
            light.Spawn();

            // WIP
            PositionConstraint positionConstraint = light.GameObject.AddComponent<PositionConstraint>();

            ConstraintSource source = new ConstraintSource
            {
                sourceTransform = pickup.Transform,
                weight = 1f
            };

            positionConstraint.AddSource(source);

            positionConstraint.translationOffset = actualOffset;
            positionConstraint.constraintActive = true;
            positionConstraint.locked = true;

            ActiveGlowEffects[pickup] = light;
        }

        private void RemoveGlowEffect(Pickup pickup)
        {
            var light = ActiveGlowEffects[pickup];
            if (light != null && light.Base != null)
            {
                NetworkServer.Destroy(light.Base.gameObject);
            }
            ActiveGlowEffects.Remove(pickup);
        }

        private void ClearAllGlowEffects()
        {
            foreach (var light in ActiveGlowEffects
                         .Select(lights => lights.Value)
                         .Where(light => light != null && light.Base != null))
            {

                NetworkServer.Destroy(light.Base.gameObject);

            }
            ActiveGlowEffects.Clear();
        }
    }
}