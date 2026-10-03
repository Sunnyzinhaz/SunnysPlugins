
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using System;
using MEC;
using YamlDotNet.Serialization;
using PlayerRoles;

namespace CustomClasses.Handlers
{

    public class PlayerHandler
    {
        private static Plugin plugin = Plugin.Singleton;
        private static Roles.ClassE ClassEModifiers = Plugin.Singleton.Config.ClassE;
        private static Roles.MajorScientist MajorScientstModifiers = Plugin.Singleton.Config.MajorScientist;
        private static Roles.GuardLeader GuardLeaderModifiers = Plugin.Singleton.Config.GuardLeader;
        private static Roles.RRT RRTModifiers = Plugin.Singleton.Config.RRT;
        //private static Roles.SerpentsHand SerpentsHandModifiers = Plugin.Singleton.Config.SerpentsHand;
        private static Roles.FatClassD FatClassDModifires = Plugin.Singleton.Config.FatClassD;
        private static Roles.BuffedClassD BuffedClassDModifiers = Plugin.Singleton.Config.BuffedClassD;
        private static Roles.JanitorClassD JanitorClassDModifiers = Plugin.Singleton.Config.JanitorClassD;  
        private static Roles.LabRatClassD LabRatClassDModifiers = Plugin.Singleton.Config.LabRatClassD;
        //private List<Player> serpentsSpawned = new List<Player>();
        private List<float> timesUsed = new List<float>();
        private CoroutineHandle CoroutineHandle;

        [YamlIgnore]
        public Dictionary<Player, DateTime> LastUsed { get; } = new();
        [YamlIgnore]
        public float Cooldown { get; set; } = 60f;

        /*public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (API.IsClassE(ev.Player) || API.IsSerpentsHand(ev.Player) && ev.Item.Type == ItemType.Adrenaline)
            {
                ev.Player.AddAhp(40f, 75f, 1.2f, 1f, 0, false);
            }
        }*/


        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Extensions.Extensions.DestroyClasses(ev.Player);

            if (ev.Player == null || ev.Reason != SpawnReason.RoundStart) return;
            if (ev.NewRole == RoleTypeId.ClassD && ClassEModifiers.ClasseCount >= Extensions.Extensions.classECount.Count && plugin.rng.Next(100) <= ClassEModifiers.ClassEChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnClassE(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.FacilityGuard && RRTModifiers.RRTCount >= Extensions.Extensions.RRTCount.Count && plugin.rng.Next(100) <= RRTModifiers.RRTChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnRRT(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.Scientist && MajorScientstModifiers.MajorCount >= Extensions.Extensions.majorScientistCount.Count && plugin.rng.Next(100) <= MajorScientstModifiers.MajorScientistChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnMajorScientist(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.FacilityGuard && GuardLeaderModifiers.LeaderCount >= Extensions.Extensions.guardLeaderCount.Count && plugin.rng.Next(100) <= GuardLeaderModifiers.GuardLeaderChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnGuardLeader(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.ClassD && FatClassDModifires.FatClassDCount >= Extensions.Extensions.fatClassDCount.Count && plugin.rng.Next(100) <= FatClassDModifires.FatClassDChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnFatClassD(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.ClassD && BuffedClassDModifiers.BuffedClassDCount >= Extensions.Extensions.buffedClassDCount.Count && plugin.rng.Next(100) <= BuffedClassDModifiers.BuffedClassDChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnBuffedClassD(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.ClassD && JanitorClassDModifiers.JanitorClassDCount >= Extensions.Extensions.JanitorClassDCount.Count && plugin.rng.Next(100) <= JanitorClassDModifiers.JanitorClassDChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnJanitorClassD(ev.Player);
                });
            }
            else if (ev.NewRole == RoleTypeId.ClassD && LabRatClassDModifiers.LabRatClassDCount >= Extensions.Extensions.LabRatClassDCount.Count && plugin.rng.Next(100) <= LabRatClassDModifiers.LabRatClassDChance)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Extensions.Extensions.SpawnLabRatClassD(ev.Player);
                });
            }
        }


        public void OnKilling(DyingEventArgs ev)
        {
            if (ev.Attacker != null && API.IsClassE(ev.Attacker) && ev.Attacker.Health <= 110)
            {
                ev.Attacker.EnableEffect(EffectType.MovementBoost);
                ev.Attacker.ChangeEffectIntensity(EffectType.MovementBoost, 10, 10);
                ev.Attacker.Heal(10f, true);
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            Extensions.Extensions.DestroyClasses(ev.Player);
        }

        /*public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null) return;
            //if (API.IsSerpentsHand(ev.Player) && ev.Attacker.IsScp || Scp035Item.ChangedPlayers.Contains(ev.Player) && ev.Attacker.IsScp)
            if (API.IsSerpentsHand(ev.Player) && ev.Attacker.IsScp)
            {
                ev.Attacker.ShowHint("Você não pode atacar seus aliados!");
                ev.IsAllowed = false;
            }
            //if (API.IsSerpentsHand(ev.Attacker) && ev.Player.IsScp || Scp035Item.ChangedPlayers.Contains(ev.Attacker) && ev.Player.IsScp)
            if (API.IsSerpentsHand(ev.Attacker) && ev.Player.IsScp)
            {
                ev.Attacker.ShowHint("Você não pode atacar seus aliados!");
                ev.IsAllowed = false;
            }
            if (ev.Attacker != null && CustomItems.Items.Scp035Item.ChangedPlayers.Contains(ev.Attacker) && API.IsSerpentsHand(ev.Player))
            {
                ev.IsAllowed = false;
            }

            if (ev.Attacker != null && CustomItems.Items.Scp035Item.ChangedPlayers.Contains(ev.Player) && API.IsSerpentsHand(ev.Attacker))
            {
                ev.IsAllowed = false;
            }
           
        }*/

        public void EnteringPocket(EnteringPocketDimensionEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial)
            {
                ev.IsAllowed = false;
                ev.Scp106.ShowHint("Você não pode mandar seus aliados para a pocket!");
            }
        }

        /*public IEnumerator<float> serpentsHandSpawn()
        {
            while (Round.IsStarted)
            {
                if (serpentsSpawned.Count() >= SerpentsHandModifiers.SerpentCount) 
                    break;

                if (Round.IsEnded) 
                    break;

                var deadPlayers = Player.List.Where(p => p.IsDead);
                if (deadPlayers.Count() >= 1)
                {
                    Log.Debug("1");
                    var serpentsCandidates = deadPlayers.ToList();
                    var selectedSerpents = serpentsCandidates.Take(2);
                    foreach (Player player in selectedSerpents)
                    {
                        if (serpentsSpawned.Contains(player))
                        {
                            Log.Debug("List already contains player, breaking yield...");
                            break;
                        }
                        Extensions.Extensions.SpawnSerpentsHand(player);
                        serpentsSpawned.Add(player);
                        Log.Debug("Spawning Serpents");
                    }
                }
                yield return Timing.WaitForSeconds(10f);
            }
        }*/
    }
}
