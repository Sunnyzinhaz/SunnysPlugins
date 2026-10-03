using System.Collections.Generic;
using System.ComponentModel;

namespace CustomClasses.Roles
{
    public class ClassE
    {
        [Description("O nome do Classe-E")]
        public string ClassEName { get; set; } = "ClassE";

        [Description("A chance de um Classe-E spawnar no incio da partida")]
        public int ClassEChance { get; set; } = 20;

        [Description("O tanto de vida que um Classe-E terá quando spawnar")]
        public float ClassEHealth = 85;

        [Description("O tanto de escudo que um Classe-E terá quando spawnar")]
        public float ClassEShield = 0f;

        [Description("Quantos ClassE podem spawnar em uma partida")]
        public float ClasseCount = 2f;

        [Description("A mensagem que aparece quando você spawna de Classe-E ")]
        public string ClassESpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#FAFB00>ê</color> <color=#F9F900>é</color> <color=#F9F700>u</color><color=#F9F600>m</color> <color=#00EEFF>C</color><color=#00E3FB>l</color><color=#00D9F7>a</color><color=#00CFF3>s</color><color=#00C5EF>s</color><color=#00BBEB>e</color><color=#00B1E7>-</color><color=#00A7E3>E</color><color=#F8F400>,</color> <color=#F8F200>v</color><color=#F8F100>o</color><color=#F7F000>c</color><color=#F7EF00>ê</color> <color=#F7ED00>t</color><color=#F6EC00>e</color><color=#F6EB00>m</color> <color=#F6E900>u</color><color=#F5E800>m</color><color=#F5E700>a</color> <color=#51FF00>v</color><color=#3CDF03>i</color><color=#28BF07>d</color><color=#149F0B>a</color> <color=#F5E400>m</color><color=#F4E300>e</color><color=#F4E200>n</color><color=#F4E100>o</color><color=#F4E000>r</color><color=#F3DF00>.</color> <color=#F3DD00>M</color><color=#F3DC00>a</color><color=#F3DB00>s</color> <color=#F2D900>g</color><color=#F2D800>a</color><color=#F2D700>n</color><color=#F1D600>h</color><color=#F1D500>a</color> <color=#8D33FA>e</color><color=#862CF9>f</color><color=#7F26F8>e</color><color=#781FF8>i</color><color=#7119F7>t</color><color=#6A13F6>o</color><color=#630CF6>s</color> <color=#F0D200>e</color><color=#F0D100>s</color><color=#F0D000>p</color><color=#F0CF00>e</color><color=#F0CE00>c</color><color=#EFCD00>i</color><color=#EFCC00>a</color><color=#EFCB00>i</color><color=#EFCA00>s</color> <color=#EEC800>a</color><color=#EEC700>o</color> <color=#EDC500>m</color><color=#EDC400>a</color><color=#EDC300>t</color><color=#EDC200>a</color><color=#EDC100>r</color> <color=#ECBF00>a</color><color=#ECBE00>l</color><color=#ECBD00>g</color><color=#EBBC00>u</color><color=#EBBB00>é</color><color=#EBBA00>m</color><color=#EBB900>.</color>\n";

        [Description("Os items que um Classe-E terá quando spawnar.")]
        public List<string> ClassESpawnItems { get; set; } = new()
        {
            "Coin",
            "FlashLight"
        };
    }
}
