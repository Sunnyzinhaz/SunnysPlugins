using Exiled.API.Features;
namespace CustomClasses
{
    public class API
    {
        private static Roles.ClassE ClassEModifiers = Plugin.Singleton.Config.ClassE;
        private static Roles.MajorScientist MajorScientstModifiers = Plugin.Singleton.Config.MajorScientist;
        private static Roles.GuardLeader GuardLeaderModifiers = Plugin.Singleton.Config.GuardLeader;
        private static Roles.RRT RRTModifiers = Plugin.Singleton.Config.RRT;
        //private static Roles.SerpentsHand SerpentsHandModifiers = Plugin.Singleton.Config.SerpentsHand;
        private static Roles.FatClassD FatClassDModifiers = Plugin.Singleton.Config.FatClassD;
        private static Roles.BuffedClassD buffedClassDModifiers = Plugin.Singleton.Config.BuffedClassD;
        private static Roles.JanitorClassD janitorClassDModifiers = Plugin.Singleton.Config.JanitorClassD;
        private static Roles.LabRatClassD labRatClassDModifiers = Plugin.Singleton.Config.LabRatClassD;
        //public static bool IsSerpentsHand(Player player) => player.SessionVariables.ContainsKey(SerpentsHandModifiers.SerpentsHandName);
        public static bool IsClassE(Player player) => player.SessionVariables.ContainsKey(ClassEModifiers.ClassEName);
        public static bool IsMajorScientist(Player player) => player.SessionVariables.ContainsKey(MajorScientstModifiers.MajorScientistName);
        public static bool IsGuardLeader(Player player) => player.SessionVariables.ContainsKey(GuardLeaderModifiers.GuardLeaderName);
        public static bool IsRRT(Player player) => player.SessionVariables.ContainsKey(RRTModifiers.RRTName);

        public static bool IsFatClassD(Player player) => player.SessionVariables.ContainsKey(FatClassDModifiers.FatClassDName);
        public static bool IsBuffedClassD(Player player) => player.SessionVariables.ContainsKey(buffedClassDModifiers.BuffedClassDName);
        public static bool IsJanitorClassD(Player player) => player.SessionVariables.ContainsKey(janitorClassDModifiers.JanitorClassDName);
        public static bool IsLabRatClassD(Player player) => player.SessionVariables.ContainsKey(labRatClassDModifiers.LabRatClassDName);
    }
}
