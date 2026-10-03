using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using MEC;
using RueI.API;
using RueI.API.Elements;

namespace Scp035.Managers
{
    internal sealed class Scp035HintManager
    {
        private readonly Plugin plugin;
        private readonly Tag pickupTag = new("Scp035PickupHint");
        private readonly Tag holdingTag = new("Scp035HoldingHint");
        private readonly Tag possessedSpectatorTag = new("Scp035PossessedSpectatorHint");
        private readonly HashSet<Player> holdingPlayers = new();
        private CoroutineHandle monitorCoroutine;

        internal Scp035HintManager(Plugin plugin)
        {
            this.plugin = plugin;
        }

        internal void Start()
        {
            monitorCoroutine = Timing.RunCoroutine(MonitorHeldItem(), "scp035-holding-hint");
        }

        internal void Stop()
        {
            Timing.KillCoroutines(monitorCoroutine);

            foreach (Player player in holdingPlayers)
            {
                if (player == null || !player.IsConnected)
                    continue;

                try
                {
                    RueDisplay.Get(player).Remove(holdingTag);
                }
                catch
                {
                    // RueI pode já ter removido o display durante desconexão/restart.
                }
            }

            holdingPlayers.Clear();
        }

        internal void ShowPickupHint(Player player)
        {
            if (player == null || !player.IsConnected || string.IsNullOrWhiteSpace(plugin.Config.PickupHint) || plugin.Config.PickupHintDuration <= 0f)
                return;

            RueDisplay.Get(player).Show(
                pickupTag,
                new BasicElement(plugin.Config.PickupHintPosition, plugin.Config.PickupHint),
                Math.Max(0.1f, plugin.Config.PickupHintDuration));
        }


        internal void ShowPossessedSpectatorHint(Player controller, Player host)
        {
            if (controller == null || host == null || !controller.IsConnected ||
                string.IsNullOrWhiteSpace(plugin.Config.PossessedSpectatorHint))
                return;

            string hostName = string.IsNullOrWhiteSpace(host.Nickname) ? "um jogador" : host.Nickname;
            string text = plugin.Config.PossessedSpectatorHint.Replace("{player}", hostName);

            RueDisplay.Get(controller).Show(
                possessedSpectatorTag,
                new BasicElement(plugin.Config.PossessedSpectatorHintPosition, text),
                Math.Max(0.1f, plugin.Config.PossessedSpectatorHintDuration));
        }

        private IEnumerator<float> MonitorHeldItem()
        {
            while (plugin.Config.IsEnabled)
            {
                foreach (Player player in Player.List)
                {
                    if (player == null || !player.IsConnected)
                        continue;

                    bool holding035 = IsHolding035(player);
                    bool alreadyShowing = holdingPlayers.Contains(player);

                    if (holding035)
                    {
                        if (!alreadyShowing)
                        {
                            holdingPlayers.Add(player);
                            ShowHoldingHint(player);
                        }
                    }
                    else if (alreadyShowing)
                    {
                        holdingPlayers.Remove(player);
                        try
                        {
                            RueDisplay.Get(player).Remove(holdingTag);
                        }
                        catch
                        {
                            // Jogador pode ter desconectado no mesmo frame.
                        }
                    }
                }

                holdingPlayers.RemoveWhere(p => p == null || !p.IsConnected);
                yield return Timing.WaitForSeconds(Math.Max(0.1f, plugin.Config.HoldingHintCheckInterval));
            }
        }

        private bool IsHolding035(Player player)
        {
            if (player.CurrentItem == null)
                return false;

            if (!CustomItem.TryGet(player.CurrentItem, out CustomItem customItem))
                return false;

            return customItem.Id == plugin.Config.Scp035ItemId;
        }

        private void ShowHoldingHint(Player player)
        {
            if (string.IsNullOrWhiteSpace(plugin.Config.HoldingHint))
                return;

            RueDisplay.Get(player).Show(
                holdingTag,
                new BasicElement(plugin.Config.HoldingHintPosition, plugin.Config.HoldingHint));
        }
    }
}
