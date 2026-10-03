using System;
using System.Collections.Generic;
using CustomItems.Core.Interfaces;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using MEC;
using RueI.API;
using RueI.API.Elements;

namespace CustomItems.Core.Hints
{
    /// <summary>
    /// Mostra somente uma hint limpa enquanto uma SCP-500 customizada estiver na mão.
    /// Não interfere na hint própria do SCP-035, pois o 035 não implementa ICustomItemHeldHint.
    /// </summary>
    public sealed class CustomHeldHintService
    {
        private readonly Tag heldTag = new("CustomItemsHeldHint");
        private readonly Dictionary<Player, uint> showing = new();
        private CoroutineHandle monitorCoroutine;

        public void Start()
        {
            monitorCoroutine = Timing.RunCoroutine(Monitor(), "customitems-held-hints");
        }

        public void Stop()
        {
            Timing.KillCoroutines(monitorCoroutine);

            foreach (Player player in showing.Keys)
                Remove(player);

            showing.Clear();
        }

        private IEnumerator<float> Monitor()
        {
            while (global::CustomItems.CustomItems.Instance != null && global::CustomItems.CustomItems.Instance.Config.IsEnabled)
            {
                foreach (Player player in Player.List)
                {
                    if (player == null || !player.IsConnected)
                        continue;

                    if (!TryGetHint(player, out uint id, out ICustomItemHeldHint hint))
                    {
                        if (showing.ContainsKey(player))
                        {
                            Remove(player);
                            showing.Remove(player);
                        }

                        continue;
                    }

                    if (!showing.TryGetValue(player, out uint shownId) || shownId != id)
                    {
                        Remove(player);
                        Show(player, hint);
                        showing[player] = id;
                    }
                }

                var disconnected = new List<Player>();
                foreach (Player player in showing.Keys)
                {
                    if (player == null || !player.IsConnected)
                        disconnected.Add(player);
                }

                foreach (Player player in disconnected)
                    showing.Remove(player);

                yield return Timing.WaitForSeconds(0.15f);
            }
        }

        private static bool TryGetHint(Player player, out uint id, out ICustomItemHeldHint hint)
        {
            id = 0;
            hint = null;

            if (player.CurrentItem == null)
                return false;

            if (!CustomItem.TryGet(player.CurrentItem, out CustomItem customItem))
                return false;

            if (customItem is not ICustomItemHeldHint heldHint || !heldHint.HasCustomHeldHint || string.IsNullOrWhiteSpace(heldHint.HeldHint))
                return false;

            id = customItem.Id;
            hint = heldHint;
            return true;
        }

        private void Show(Player player, ICustomItemHeldHint hint)
        {
            try
            {
                RueDisplay.Get(player).Show(
                    heldTag,
                    new BasicElement(hint.HeldHintPosition, hint.HeldHint));
            }
            catch (Exception e)
            {
                Log.Debug($"[CustomItems] Falha ao mostrar holding hint: {e.Message}");
            }
        }

        private void Remove(Player player)
        {
            if (player == null || !player.IsConnected)
                return;

            try
            {
                RueDisplay.Get(player).Remove(heldTag);
            }
            catch
            {
                // Display pode já ter sido destruído ao trocar de mapa/desconectar.
            }
        }
    }
}
