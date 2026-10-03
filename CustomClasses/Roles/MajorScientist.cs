using System.Collections.Generic;
using System.ComponentModel;

namespace CustomClasses.Roles
{
    public class MajorScientist
    {
        [Description("O tanto de vida que um Cientista Major terá quando spawnar")]
        public float MajorScientistHealth = 100;

        [Description("O tanto de escudo que um Cientista Major terá quando spawnar")]
        public float MajorScientistShield = 0f;

        [Description("O nome do Cientista Major")]
        public string MajorScientistName { get; set; } = "MajorScientist";

        [Description("A chance de um Cientista Major spawnar no incio da partida")]
        public int MajorScientistChance { get; set; } = 15;

        [Description("Quantos Majors podem spawnar em uma partida")]
        public float MajorCount = 2f;

        [Description("A mensagem que aparece quando você spawna de Cientista Major")]
        public string MajorScientistMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#F9FA00>ê</color> <color=#F9F700>é</color> <color=#F8F400>u</color><color=#F8F300>m</color> <color=#00EEFF>S</color><color=#00EAFD>u</color><color=#00E6FC>p</color><color=#00E2FA>e</color><color=#00DFF9>r</color><color=#00DBF7>v</color><color=#00D7F6>i</color><color=#00D4F5>s</color><color=#00D0F3>o</color><color=#00CCF2>r</color> <color=#00C5EF>d</color><color=#00C1EE>e</color> <color=#00BAEB>P</color><color=#00B6E9>e</color><color=#00B3E8>s</color><color=#00AFE7>q</color><color=#00ABE5>u</color><color=#00A8E4>i</color><color=#00A4E2>s</color><color=#00A0E1>a</color><color=#F7F100>,</color> <color=#F7EE00>v</color><color=#F6EC00>o</color><color=#F6EB00>c</color><color=#F6EA00>ê</color> <color=#F5E700>s</color><color=#F5E500>p</color><color=#F5E400>a</color><color=#F4E300>w</color><color=#F4E100>n</color><color=#F4E000>a</color> <color=#F3DD00>c</color><color=#F3DC00>o</color><color=#F2DA00>m</color> <color=#F2D800>a</color><color=#F1D600>l</color><color=#F1D500>g</color><color=#F1D300>u</color><color=#F0D200>n</color><color=#F0D100>s</color> <color=#F0CE00>i</color><color=#EFCC00>t</color><color=#EFCB00>e</color><color=#EFCA00>m</color><color=#EEC800>s</color> <color=#EEC500>e</color><color=#EDC400>s</color><color=#EDC300>p</color><color=#EDC100>e</color><color=#ECC000>c</color><color=#ECBE00>i</color><color=#ECBD00>a</color><color=#EBBC00>i</color><color=#EBBA00>s</color><color=#EBB900>!</color>\n";

        [Description("Os items que um Cientista major terá quando spawnar.")]
        public List<string> MajorScientistSpawnItems { get; set; } = new()
        {
            "KeycardResearchCoordinator",
            "Medkit",
            "Adrenaline",
            "GunCOM15"
        };
    }
}
