using CustomItems.Core;
using CustomItems.Core.Hints;
using HarmonyLib;
#pragma warning disable SA1200
namespace CustomItems
{
    using System;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using global::CustomItems.Events;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class CustomItems : Plugin<Config>
    {
        private readonly Harmony _harmony = new Harmony("CustomItems");
        
        private ServerHandler _serverHandler;
        private GlowEventsHandler _glowEventsHandler;
        private CustomHeldHintService _customHeldHintService;
        
        public static CustomItems Instance { get; private set; } = null!;
        public override string Author { get; } = "Joker119; MMDDKK6500; Kawai; GalaxySamurai; Edi; Unbistrackted; Snivy (glow code); Sunnyz";
        public override string Name { get; } = "CustomItems";
        public override string Prefix { get; } = "CustomItems";
        public override Version RequiredExiledVersion { get; } = new(9, 8, 1);
        public override Version Version { get; } = new Version(2, 4, 4);
        

        public override void OnEnabled()
        {
            Instance = this;
            _serverHandler = new ServerHandler();
            _glowEventsHandler = new GlowEventsHandler();
            _customHeldHintService = new CustomHeldHintService();
            _customHeldHintService.Start();

            Config.LoadItems();

            Log.Debug("Registering items..");
            CustomItem.RegisterItems(overrideClass: Config.ItemConfigs);
            Server.ReloadedConfigs += _serverHandler.OnReloadingConfigs;
            Server.RoundStarted += _serverHandler.OnRoundStarted;
            Server.WaitingForPlayers += _serverHandler.OnWaitingForPlayers;
            _harmony.PatchAll();
            
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            Server.ReloadedConfigs -= _serverHandler.OnReloadingConfigs;
            Server.RoundStarted -= _serverHandler.OnRoundStarted;
            Server.WaitingForPlayers -= _serverHandler.OnWaitingForPlayers;
            _harmony.UnpatchAll();
            _customHeldHintService?.Stop();
            _customHeldHintService = null;
            _glowEventsHandler = null;
            _serverHandler = null;

            base.OnDisabled();
        }
    }
}