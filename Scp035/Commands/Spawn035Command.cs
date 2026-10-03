using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace Scp035.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public sealed class Spawn035Command : ICommand
    {
        public string Command { get; } = "035spawn";
        public string[] Aliases { get; } = { "spawn035", "scp035spawn", "give035", "scp035" };
        public string Description { get; } = "Spawna o item SCP-035 nos pés do administrador para testes ou uso manual.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin plugin = Plugin.Instance;
            if (plugin == null || !plugin.Config.IsEnabled)
            {
                response = "SCP-035 está desativado.";
                return false;
            }

            if (!sender.CheckPermission(plugin.Config.SpawnCommandPermission))
            {
                response = $"Sem permissão. Necessário: {plugin.Config.SpawnCommandPermission}";
                return false;
            }

            if (!(sender is CommandSender commandSender))
            {
                response = "Esse comando deve ser usado pelo Remote Admin dentro do jogo.";
                return false;
            }

            Player player = Player.Get(commandSender.SenderId);
            if (player == null || !player.IsConnected)
            {
                response = "Esse comando deve ser usado pelo Remote Admin dentro do jogo.";
                return false;
            }

            Vector3 position = player.Position + (Vector3.up * plugin.Config.DebugSpawnYOffset);

            if (!CustomItem.TrySpawn(plugin.Config.Scp035ItemId, position, out var pickup) || pickup == null)
            {
                response = $"Não foi possível spawnar o SCP-035. Verifique se o CustomItem ID {plugin.Config.Scp035ItemId} está registrado.";
                return false;
            }

            string room = player.CurrentRoom?.Type.ToString() ?? "Sala desconhecida";
            string zone = player.CurrentRoom?.Zone.ToString() ?? "Zona desconhecida";

            Log.Info(
                $"[SCP-035][COMMAND] {player.Nickname} usou 035spawn | " +
                $"Sala: {room} | Zona: {zone} | Posição: {position} | ID: {plugin.Config.Scp035ItemId}");

            response =
                $"SCP-035 spawnado. Sala: {room} | Zona: {zone} | " +
                $"CustomItem ID: {plugin.Config.Scp035ItemId}.";

            return true;
        }
    }
}
