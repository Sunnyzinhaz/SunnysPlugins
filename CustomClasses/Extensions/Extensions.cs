using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using System;
using System.Linq;
using CustomPlayerEffects;

namespace CustomClasses.Extensions
{
    public class Extensions
    {
        private static Roles.ClassE classEModifiers = Plugin.Singleton.Config.ClassE;
        private static Roles.MajorScientist majorScientstModifiers = Plugin.Singleton.Config.MajorScientist;
        private static Roles.GuardLeader guardLeaderModifiers = Plugin.Singleton.Config.GuardLeader;
        private static Roles.RRT RRTModifiers = Plugin.Singleton.Config.RRT;
        //private static Roles.SerpentsHand serpentsHandModifiers = Plugin.Singleton.Config.SerpentsHand;
        private static Roles.FatClassD FatClassDModifires = Plugin.Singleton.Config.FatClassD;
        private static Roles.BuffedClassD BuffedClassDModifiers = Plugin.Singleton.Config.BuffedClassD;
        private static Roles.JanitorClassD JanitorClassDModifiers = Plugin.Singleton.Config.JanitorClassD;
        private static Roles.LabRatClassD LabRatClassDModifiers = Plugin.Singleton.Config.LabRatClassD;
        public static List<Player> brutusCount = new List<Player>
        {

        };
        public static List<Player> majorScientistCount = new List<Player>
        {

        //};
        //public static List<Player> serpentsCount = new List<Player>
        //{

        };
        public static List<Player> guardLeaderCount = new List<Player>
        {

        };
        public static List<Player> RRTCount = new List<Player>
        {

        };
        public static List<Player> classECount = new List<Player>
        {

        };
        public static List<Player> fatClassDCount = new List<Player>
        {

        };
        public static List<Player> buffedClassDCount = new List<Player>
        {

        };
        public static List<Player> JanitorClassDCount = new List<Player>
        {

        };
        public static List<Player> LabRatClassDCount = new List<Player>
        {

        };

        private static Plugin plugin = Plugin.Singleton;
        private static System.Random rnd = new();

        /*public static void SpawnSerpentsHand(Player player)
        {
            player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
            player.SessionVariables.Add(serpentsHandModifiers.SerpentsHandName, null);
            player.Health = serpentsHandModifiers.SerpentsHandHealth;
            player.MaxHealth = serpentsHandModifiers.SerpentsHandHealth;
            player.AddAhp(serpentsHandModifiers.SerpentsHandShield, serpentsHandModifiers.SerpentsHandShield, 0, 1, 0, true);
            player.Position = new Vector3(0, 1003, -36);
            player.ResetInventory(serpentsHandModifiers.SerpentsHandSpawnItems);
            player.Broadcast(5, serpentsHandModifiers.SerpentsHandSpawnMessage);
            player.CustomInfo = $"Serpent's Hand";
            serpentsCount.Add(player);
        }*/

        public static void SpawnClassE(Player player)
        {
            player.SessionVariables.Add(classEModifiers.ClassEName, null);
            player.Health = classEModifiers.ClassEHealth;
            player.MaxHealth = classEModifiers.ClassEHealth;
            player.AddAhp(classEModifiers.ClassEShield, classEModifiers.ClassEShield, 0, 1);
            player.ResetInventory(classEModifiers.ClassESpawnItems);
            player.Broadcast(5, classEModifiers.ClassESpawnMessage);
            player.CustomInfo = $"Classe-E";
            player.AddAmmo(AmmoType.Nato9, 10);

            classECount.Add(player);
        }

        public static void SpawnMajorScientist(Player player)
        {

            player.SessionVariables.Add(majorScientstModifiers.MajorScientistName, null);
            player.Health = majorScientstModifiers.MajorScientistHealth;
            player.MaxHealth = majorScientstModifiers.MajorScientistHealth;
            player.AddAhp(majorScientstModifiers.MajorScientistShield, majorScientstModifiers.MajorScientistShield, 0, 1);
            player.ResetInventory(majorScientstModifiers.MajorScientistSpawnItems);
            player.Broadcast(5, majorScientstModifiers.MajorScientistMessage);
            player.CustomInfo = $"Supervisor de Pesquisa";
            player.AddAmmo(AmmoType.Nato9, 15);

            majorScientistCount.Add(player);
        }

        public static void SpawnGuardLeader(Player player)
        {
            player.SessionVariables.Add(guardLeaderModifiers.GuardLeaderName, null);
            player.Health = guardLeaderModifiers.GuardLeaderHealth;
            player.MaxHealth = guardLeaderModifiers.GuardLeaderHealth;
            player.AddAhp(guardLeaderModifiers.GuardLeaderShield, guardLeaderModifiers.GuardLeaderShield, 0, 1);
            player.ResetInventory(guardLeaderModifiers.GuardLeaderSpawnItems);
            player.Broadcast(5, guardLeaderModifiers.GuardLeaderMessage);
            player.CustomInfo = $"Guarda da Instalação Sargento";
            player.AddAmmo(AmmoType.Nato9, 100);
            guardLeaderCount.Add(player);
        }

        public static void SpawnRRT(Player player)
        {
            player.SessionVariables.Add(RRTModifiers.RRTName, null);
            player.Health = RRTModifiers.RRTHealth;
            player.MaxHealth = RRTModifiers.RRTHealth;
            player.AddAhp(RRTModifiers.RRTShield, RRTModifiers.RRTShield, 0, 1);
            player.Broadcast(5, RRTModifiers.RRTSpawnMessage);
            player.CustomInfo = $"Equipe de Resposta Rápida";
            player.ResetInventory(RRTModifiers.RRTSpawnItems);
            player.Position = SpawnLocationType.InsideIntercom.GetPosition();
            player.EnableEffect(EffectType.MovementBoost, 5);
            player.AddAmmo(AmmoType.Nato556, 80);
            player.AddAmmo(AmmoType.Nato9, 40);

            RRTCount.Add(player);
        }

        //Classe-D gordo: Dobro de vida mas -20% de velocidade de movimento e -50% de stamina
        public static void SpawnFatClassD(Player player)
        {
            player.SessionVariables.Add(FatClassDModifires.FatClassDName, null);
            player.Broadcast(5, FatClassDModifires.FatClassDSpawnMessage);
            player.Health = FatClassDModifires.FatClassDHealth;
            player.MaxHealth = FatClassDModifires.FatClassDHealth;
            player.EnableEffect(Exiled.API.Enums.EffectType.Exhausted);
            player.CustomInfo = $"Classe-D Gordo";
            player.AddAmmo(AmmoType.Nato9, 10);

            fatClassDCount.Add(player);
        }
        
        //Classe-D bombado: é crescido, tem 150 de vida e +50% de stamina
        public static void SpawnBuffedClassD(Player player)
        {
            player.SessionVariables.Add(BuffedClassDModifiers.BuffedClassDName, null);
            player.Broadcast(5, BuffedClassDModifiers.BuffedClassDSpawnMessage);
            player.Health = BuffedClassDModifiers.BuffedClassDHealth;
            player.MaxHealth = BuffedClassDModifiers.BuffedClassDHealth;
            //Halve the player's stamina(???)
            //player.EnableEffect(Exiled.API.Enums.EffectType.Exhausted);
            player.CustomInfo = $"Classe-D Bodybuilder";
            player.AddAmmo(AmmoType.Nato9, 10);

            buffedClassDCount.Add(player);
        }

        public static void SpawnJanitorClassD(Player player)
        {
            player.SessionVariables.Add(JanitorClassDModifiers.JanitorClassDName, null);
            player.Broadcast(5, JanitorClassDModifiers.JanitorClassDSpawnMessage);
            player.Health = JanitorClassDModifiers.JanitorClassDHealth;
            player.MaxHealth = JanitorClassDModifiers.JanitorClassDHealth;
            player.Position = SpawnLocationType.InsideLczWc.GetPosition();
            player.ResetInventory(JanitorClassDModifiers.JanitorClassDSpawnItems);
            player.CustomInfo = $"Faxineiro";
            player.AddAmmo(AmmoType.Nato9, 10);

            JanitorClassDCount.Add(player);
        }

        public static void SpawnLabRatClassD(Player player)
        {
            player.SessionVariables.Add(LabRatClassDModifiers.LabRatClassDName, null);
            player.Broadcast(5, LabRatClassDModifiers.LabRatClassDSpawnMessage);
            player.Health = LabRatClassDModifiers.LabRatClassDHealth;
            player.MaxHealth = LabRatClassDModifiers.LabRatClassDHealth;
            player.Position = SpawnLocationType.InsideGr18.GetPosition();
            List<string> Item =
            [
                LabRatClassDModifiers.LabRatClassDSpawnItems.ElementAt(rnd.Next(LabRatClassDModifiers.LabRatClassDSpawnItems.Count - 1)),
                "Coin",

            ];
            player.ResetInventory(Item);
            player.CustomInfo = $"Classe-D Rato de Laboratório";
            player.AddAmmo(AmmoType.Nato9, 10);

            LabRatClassDCount.Add(player);
        }


        public static void DestroyClasses(Player player)
        {
            RemoveSessionVariables(player);
            ResetSettings(player);
        }

        public static void RemoveSessionVariables(Player player)
        {
            player.SessionVariables.Remove(classEModifiers.ClassEName);
            player.SessionVariables.Remove(majorScientstModifiers.MajorScientistName);
            //player.SessionVariables.Remove(serpentsHandModifiers.SerpentsHandName);
            player.SessionVariables.Remove(guardLeaderModifiers.GuardLeaderName);
            player.SessionVariables.Remove(RRTModifiers.RRTName);
            player.SessionVariables.Remove(FatClassDModifires.FatClassDName);
            player.SessionVariables.Remove(BuffedClassDModifiers.BuffedClassDName);
            player.SessionVariables.Remove(JanitorClassDModifiers.JanitorClassDName);
            player.SessionVariables.Remove(LabRatClassDModifiers.LabRatClassDName);

        }

        public static void ResetSettings(Player player)
        {
            player.MaxHealth = default;
            player.Health = default;
            player.CustomInfo = string.Empty;
        }
    }
}
