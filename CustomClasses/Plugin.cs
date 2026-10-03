using Exiled.API.Features;
using System;
using CustomClasses.Handlers;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace CustomClasses
{
    public class Plugin : Plugin<Configs.Config>
    {
        public static Plugin Singleton;

        /// <inheritdoc />
        public override string Author { get; } = "Gal; MMDDKK6500, Darkness_Soul7, Sunnyz";

        /// <inheritdoc />
        public override string Name { get; } = "CustomClasses";

        /// <inheritdoc />
        public override string Prefix { get; } = "CustomClasses";

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new(8, 9, 5);
        /// <inheritdoc />
        public override Version Version { get; } = new Version(2, 1, 0);

        public Random rng = new();

        public PlayerHandler player;

        public override void OnEnabled()
        {
            Singleton = this;

            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnRegisterEvents();
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            player = new PlayerHandler();

            Player.ChangingRole += player.OnChangingRole;
            Player.Dying += player.OnKilling;
            Player.Dying += player.OnDying;
            //Player.Hurting += player.OnHurting;
            Player.EnteringPocketDimension += player.EnteringPocket;
        }

        private void UnRegisterEvents()
        {
            Player.ChangingRole -= player.OnChangingRole;
            Player.Dying -= player.OnKilling;
            Player.Dying -= player.OnDying;
            //Player.Hurting -= player.OnHurting;
            Player.EnteringPocketDimension -= player.EnteringPocket;
            player = null;
        }

        /* Add:
        //Classe-D gordo: Dobro de vida mas -20% de velocidade de movimento e -50% de stamina
        //Classe-D bombado: é crescido, tem 150 de vida e +50% de stamina
        //Classe-D faxineiro: Spawna fora da CDC e começa com cartão de faxineiro e 2 moedas
        //Classe-D rato de laboratorio: Spawna na GC18 com um item SCP
        */
    }
}
