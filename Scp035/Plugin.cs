using System;
using Exiled.API.Features;
using Scp035.Managers;
using PlayerEvents = Exiled.Events.Handlers.Player;
using ServerEvents = Exiled.Events.Handlers.Server;
using Scp096Events = Exiled.Events.Handlers.Scp096;
using Scp173Events = Exiled.Events.Handlers.Scp173;

namespace Scp035
{
    public sealed class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Author { get; } = "MMDDKK6500; Sunnyz";
        public override string Name { get; } = "Scp035";
        public override string Prefix { get; } = "Scp035";
        public override Version RequiredExiledVersion { get; } = new(9, 14, 2);
        public override Version Version { get; } = new(3, 20, 17);

        internal Scp035Manager Manager { get; private set; }
        internal Scp035HintManager Hints { get; private set; }
        internal EventHandlers Events { get; private set; }

        public static bool IsScp035Player(Player player) => Instance?.Manager?.Is035(player) == true;

        public override void OnEnabled()
        {
            Instance = this;

            Manager = new Scp035Manager(this);
            Hints = new Scp035HintManager(this);
            Events = new EventHandlers(this, Manager);

            Hints.Start();

            PlayerEvents.SearchingPickup += Events.OnSearchingPickup;
            PlayerEvents.PickingUpItem += Events.OnPickingUpItem;
            PlayerEvents.UsingItem += Events.OnUsingItem;
            PlayerEvents.Transmitting += Events.OnTransmitting;
            PlayerEvents.Hurting += Events.OnHurting;
            PlayerEvents.Died += Events.OnDied;
            PlayerEvents.SpawningRagdoll += Events.OnSpawningRagdoll;

            Scp096Events.AddingTarget += Events.On096AddingTarget;
            Scp173Events.AddingObserver += Events.On173AddingObserver;
            Scp173Events.BeingObserved += Events.On173BeingObserved;

            ServerEvents.RoundStarted += Events.OnRoundStarted;
            ServerEvents.EndingRound += Events.OnEndingRound;
            ServerEvents.WaitingForPlayers += Events.OnWaitingForPlayers;
            ServerEvents.RestartingRound += Events.OnRestartingRound;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerEvents.SearchingPickup -= Events.OnSearchingPickup;
            PlayerEvents.PickingUpItem -= Events.OnPickingUpItem;
            PlayerEvents.UsingItem -= Events.OnUsingItem;
            PlayerEvents.Transmitting -= Events.OnTransmitting;
            PlayerEvents.Hurting -= Events.OnHurting;
            PlayerEvents.Died -= Events.OnDied;
            PlayerEvents.SpawningRagdoll -= Events.OnSpawningRagdoll;

            Scp096Events.AddingTarget -= Events.On096AddingTarget;
            Scp173Events.AddingObserver -= Events.On173AddingObserver;
            Scp173Events.BeingObserved -= Events.On173BeingObserved;

            ServerEvents.RoundStarted -= Events.OnRoundStarted;
            ServerEvents.EndingRound -= Events.OnEndingRound;
            ServerEvents.WaitingForPlayers -= Events.OnWaitingForPlayers;
            ServerEvents.RestartingRound -= Events.OnRestartingRound;

            Hints?.Stop();
            Manager?.Reset(true);

            Events = null;
            Hints = null;
            Manager = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
