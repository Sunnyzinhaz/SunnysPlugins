using System;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Core;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Lockers;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using MEC;
using UnityEngine;
using static CustomItems.CustomItems;

namespace CustomItems.Events
{
    public class ServerHandler
    {
        private CoroutineHandle spawnRoutine;
        private static readonly uint[] Scp500Ids = { 1, 2, 3, 4, 5, 6, 7, 8 };

        public void OnReloadingConfigs()
        {
            Instance.Config.LoadItems();
        }

        public void OnRoundStarted()
        {
            CustomItemUseGate.Clear();
            Timing.KillCoroutines(spawnRoutine);
            spawnRoutine = Timing.RunCoroutine(SpawnRoundItems());
        }

        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(spawnRoutine);
            CustomItemUseGate.Clear();
        }

        private IEnumerator<float> SpawnRoundItems()
        {
            yield return Timing.WaitForSeconds(2.5f);

            SpawnScp035();

            if (!Instance.Config.EnableNaturalScp500Spawn)
            {
                Log.Warn("[CustomItems][PERF TEST] Spawn natural das SCP-500 DESATIVADO. Nenhum armário será pesquisado e nenhuma SCP-500 será criada automaticamente.");
                yield break;
            }

            int copies = Math.Max(0, Instance.Config.Scp500CopiesPerType);
            float typeChance = Mathf.Clamp(Instance.Config.Scp500SpawnChancePerType, 0f, 100f);

            foreach (uint id in Scp500Ids)
            {
                for (int i = 0; i < copies; i++)
                {
                    if (UnityEngine.Random.Range(0f, 100f) > typeChance)
                        continue;

                    SpawnScp500InLockerOnly(id, $"SCP-500 id {id}");
                }
            }
        }

        private bool SpawnScp500InLockerOnly(uint customItemId, string logName)
        {
            if (TrySpawnInRandomLocker(customItemId, out Pickup lockerPickup, out Locker locker))
            {
                Log.Info($"[{logName}] Spawn raro em ARMÁRIO | Zona: {locker.Zone} | Sala: {locker.Room?.Type} | Posição: {lockerPickup.Position}");
                return true;
            }

            // Intencionalmente NÃO existe fallback para o chão.
            Log.Debug($"[{logName}] Nenhum armário válido disponível nesta rodada; item não será spawnado.");
            return false;
        }

        private void SpawnScp035()
        {
            const uint id = 99;

            if (Pickup.List.Any(p => CustomItem.TryGet(p, out CustomItem ci) && ci.Id == id))
                return;

            if (Player.List.Any(pl => pl.Items.Any(i => CustomItem.TryGet(i, out CustomItem ci) && ci.Id == id)))
                return;

            CustomItem item = CustomItem.Get(id);
            float roundChance = 100f;

            try
            {
                var prop = item?.GetType().GetProperty("RoundSpawnChance");
                if (prop?.GetValue(item) is float f)
                    roundChance = Mathf.Clamp(f, 0f, 100f);
            }
            catch { }

            if (UnityEngine.Random.Range(0f, 100f) > roundChance)
            {
                Log.Info($"[SCP-035] Spawn natural não ocorreu nesta rodada (chance: {roundChance:0.##}%).");
                return;
            }

            SpawnRandom(id, Instance.Config.Scp035LockerSpawnChance, "SCP-035");
        }

        private bool SpawnRandom(uint customItemId, float lockerChance, string logName)
        {
            lockerChance = Mathf.Clamp(lockerChance, 0f, 100f);
            bool preferLocker = UnityEngine.Random.Range(0f, 100f) < lockerChance;

            if (preferLocker && TrySpawnInRandomLocker(customItemId, out Pickup lockerPickup, out Locker locker))
            {
                Log.Info($"[{logName}] Spawn em ARMÁRIO | Zona: {locker.Zone} | Sala: {locker.Room?.Type} | Posição: {lockerPickup.Position}");
                return true;
            }

            if (TrySpawnInRandomRoom(customItemId, out Pickup roomPickup, out Room room))
            {
                Log.Info($"[{logName}] Spawn no MAPA | Zona: {room.Zone} | Sala: {room.Type} | Posição: {roomPickup.Position}");
                return true;
            }

            if (!preferLocker && TrySpawnInRandomLocker(customItemId, out lockerPickup, out locker))
            {
                Log.Info($"[{logName}] Spawn fallback em ARMÁRIO | Zona: {locker.Zone} | Sala: {locker.Room?.Type} | Posição: {lockerPickup.Position}");
                return true;
            }

            Log.Warn($"[{logName}] Não foi possível encontrar um local válido de spawn.");
            return false;
        }

        private bool TrySpawnInRandomLocker(uint customItemId, out Pickup pickup, out Locker locker)
        {
            pickup = null;
            locker = null;

            List<Locker> lockers = Locker.List
                .Where(l => l != null &&
                            l.Chambers != null &&
                            l.Chambers.Count > 0 &&
                            l.Room != null &&
                            l.Room.Type != RoomType.EzShelter &&
                            (l.Zone == ZoneType.LightContainment ||
                             l.Zone == ZoneType.HeavyContainment ||
                             l.Zone == ZoneType.Entrance))
                .OrderBy(_ => UnityEngine.Random.value)
                .ToList();

            foreach (Locker candidate in lockers)
            {
                try
                {
                    if (!CustomItem.TrySpawn(customItemId, candidate.RandomChamberPosition, out Pickup created) || created == null)
                        continue;

                    candidate.AddItem(created);

                    pickup = created;
                    locker = candidate;
                    return true;
                }
                catch (Exception e)
                {
                    Log.Debug($"[CustomItems] Falha não crítica tentando armário aleatório: {e.Message}");
                }
            }

            return false;
        }

        private bool TrySpawnInRandomRoom(uint customItemId, out Pickup pickup, out Room room)
        {
            pickup = null;
            room = null;

            List<Room> rooms = Room.List
                .Where(r => r != null &&
                            r.Type != RoomType.EzShelter &&
                            (r.Zone == ZoneType.LightContainment ||
                             r.Zone == ZoneType.HeavyContainment ||
                             r.Zone == ZoneType.Entrance))
                .OrderBy(_ => UnityEngine.Random.value)
                .ToList();

            float radius = Mathf.Clamp(Instance.Config.RandomGroundSpawnRadius, 0f, 1f);

            foreach (Room candidate in rooms)
            {
                try
                {
                    Vector2 jitter = UnityEngine.Random.insideUnitCircle * radius;
                    Vector3 position = candidate.Position + new Vector3(jitter.x, 0.20f, jitter.y);

                    if (!CustomItem.TrySpawn(customItemId, position, out Pickup created) || created == null)
                        continue;

                    pickup = created;
                    room = candidate;
                    return true;
                }
                catch (Exception e)
                {
                    Log.Debug($"[CustomItems] Falha não crítica tentando sala aleatória: {e.Message}");
                }
            }

            return false;
        }
    }
}
