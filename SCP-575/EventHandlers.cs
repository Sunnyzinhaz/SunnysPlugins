using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Security.Policy;

using CassieExiled = Exiled.API.Features.Cassie;

namespace SCP_575
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        private readonly Random Rng = new Random();
        private CoroutineHandle Coroutine;
        private bool Scp575 = false;
        private bool Scp079 = false;

        public void OnRoundStarted()
        {
            if (Rng.Next(100) <= plugin.Config.SpawnChance)
            {
                Scp575 = true;
                Coroutine = Timing.RunCoroutine(Release575());
            }
        }
        public void OnRoundEnded(RoundEndedEventArgs ev) =>
            Timing.KillCoroutines(Coroutine);
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Tesla.Room.AreLightsOff && plugin.Config.DisableTeslasDuringBlackout && Scp575)
            {
                ev.IsInIdleRange = false;
                ev.IsAllowed = false;
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleTypeId.Scp079)
                Scp079 = true;
        }
        public void OnDamagingWindow(DamagingWindowEventArgs ev)
        {
            if (!Scp079 && Scp575 && ev.Window.Type == GlassType.Scp079Trigger)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    CassieExiled.MessageTranslated(plugin.Config.RecontainedCassieNo079.Message, plugin.Config.RecontainedCassieNo079.Translation);
                    Timing.KillCoroutines(Coroutine);
                    Scp575 = false;
                });
            }
        }
        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.Role.Type == RoleTypeId.Scp079 && Scp575)
            {
                ev.IsAllowed = false;
                CassieExiled.MessageTranslated(plugin.Config.RecontainedCassie.Message, plugin.Config.RecontainedCassie.Translation);
                Timing.KillCoroutines(Coroutine);
                Scp575 = false;
                Scp079 = false;
            }
        }

        private IEnumerator<float> Release575()
        {

            yield return Timing.WaitForSeconds(plugin.Config.InitialBlackoutDelay);

            //First blackout doesn't affect LCZ

            CassieExiled.MessageTranslated(plugin.Config.BlackoutCassie.Message, plugin.Config.BlackoutCassie.Translation);
            yield return Timing.WaitForSeconds(CassieExiled.CalculateDuration(plugin.Config.BlackoutCassie.Message));

            yield return Timing.WaitUntilDone(Timing.RunCoroutine(Blackout(plugin.Config.BlackoutDuration, true)));

            CassieExiled.MessageTranslated(plugin.Config.BlackoutEndCassie.Message, plugin.Config.BlackoutEndCassie.Translation);
            yield return Timing.WaitForSeconds(CassieExiled.CalculateDuration(plugin.Config.BlackoutEndCassie.Message) - 2f);

            yield return Timing.WaitForSeconds(plugin.Config.BlackoutDelay);

            //Other blackouts affect LCZ

            while (true)
            {
                CassieExiled.MessageTranslated(plugin.Config.BlackoutCassie.Message, plugin.Config.BlackoutCassie.Translation);
                yield return Timing.WaitForSeconds(CassieExiled.CalculateDuration(plugin.Config.BlackoutCassie.Message));

                yield return Timing.WaitUntilDone(Timing.RunCoroutine(Blackout(plugin.Config.BlackoutDuration, false)));

                CassieExiled.MessageTranslated(plugin.Config.BlackoutEndCassie.Message, plugin.Config.BlackoutEndCassie.Translation);
                yield return Timing.WaitForSeconds(CassieExiled.CalculateDuration(plugin.Config.BlackoutEndCassie.Message) - 2f);

                yield return Timing.WaitForSeconds(plugin.Config.BlackoutDelay);
            }
        }
        private IEnumerator<float> Blackout(float duration, bool isFirstBlackout = false)
        {   
            List<ZoneType> zones = [ZoneType.Unspecified];
            if (isFirstBlackout)
                zones = [ZoneType.HeavyContainment, ZoneType.Entrance, ZoneType.Surface];
            Map.TurnOffAllLights(duration, zones);
            yield return Timing.WaitForSeconds(1f);

            while ((duration -= plugin.Config.DamageInterval) > plugin.Config.DamageInterval)
            {
                foreach (Player ply in Player.List)
                    if (ply.CurrentRoom.AreLightsOff && !HasLightSource(ply) && ply.IsHuman)
                    {
                        ply.ShowHint(plugin.Config.BeingDamagedHint, plugin.Config.DamageInterval);
                        ply.Hurt(plugin.Config.DamageAmount, plugin.Config.DeathMessage);
                    }

                yield return Timing.WaitForSeconds(plugin.Config.DamageInterval);
            }
            yield break;
        }

        private bool HasLightSource(Player ply) =>
            (ply.CurrentItem is Flashlight flashlight && flashlight.IsEmittingLight) || ply.HasFlashlightModuleEnabled;

    }
}