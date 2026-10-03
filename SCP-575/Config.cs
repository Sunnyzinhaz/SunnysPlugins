using Exiled.API.Interfaces;
using SCP_575.Structs;
using System.ComponentModel;

namespace SCP_575
{
    public class Config : IConfig
    {
        [Description("Indicates if the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates the probability of SCP-575 of appearing in game.")]
        public float SpawnChance { get; set; } = 30f;

        [Description("Indicates the time for the first blackout.")]
        public float InitialBlackoutDelay { get; set; } = 300;

        [Description("Indicates the time for the next blackouts.")]
        public float BlackoutDelay { get; set; } = 250;

        [Description("Indicates the duration of the blackouts.")]
        public float BlackoutDuration { get; set; } = 30f;

        [Description("Indicates the cassie message for when a blackout occurs")]
        public CassieStruct BlackoutCassie { get; set; } = new CassieStruct(
            "Warning . Site 0 pitch_0.6 .g6 pitch_1 Light System Failure Detected  pitch_0.8 .g2 pitch_1 Initializing Automatic Repair jam_40_9 pitch_0.6 .g4",
            "Atenção, falha no sistema de energia detectada, Inicializando reparo automatico.");

        [Description("Indicates the cassie message for when a blackout ends")]
        public CassieStruct BlackoutEndCassie { get; set; } = new CassieStruct(
            ".g6 Warning . Site 0 pitch_0.5 .g4 pitch_1 Light System Is Now jam_20_9 Operational .g3",
            "Atenção, sistema de energia agora está operacional.");

        [Description("Indicates the amount of damage dealt")]
        public float DamageAmount { get; set; } = 15f;

        [Description("Indicates the delay between damage ticks")]
        public float DamageInterval { get; set; } = 10f;

        [Description("Indicates the hint someone receive when being damage by SCP-575")]
        public string BeingDamagedHint { get; set; } = "<b><color=#ff0000ff>[Atenção]</color></b>\n<i><color=#008080ff>Você foi atingido pelo</color> <color=#ff0000ff>SCP-575!</color> <color=#008080ff>Equipe uma lanterna</color></i>";

        [Description("Indicates the death message for when SCP-575 kill someone.")]
        public string DeathMessage { get; set; } = "Você morreu para: SCP-575. Tome cuidado! Equipe uma lanterna na próxima vez!";

        [Description("Indicates if tesla gates should activate when the lights are off.")]
        public bool DisableTeslasDuringBlackout { get; set; } = true;

        [Description("Indicates the cassie message for when SCP-575 is contained and there's a SCP-079 in game.")]
        public CassieStruct RecontainedCassie { get; set; } = new CassieStruct(
            "SCP 5 7 5 and SCP 0 7 9 ContainedSuccessfully",
            "SCP-575 e SCP-079 contenção sucedida");

        [Description("Indicates the cassie message for when SCP-575 is contained and there's no SCP-079 in game.")]
        public CassieStruct RecontainedCassieNo079 { get; set; } = new CassieStruct(
            "SCP 5 7 5 ContainedSuccessfully",
            "SCP-575 contenção sucedida");

        [Description("Testing mode. (does nothing)")]
        public bool Debug { get; set; } = false;
    }
}