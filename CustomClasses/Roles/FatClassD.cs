using System.Collections.Generic;
using System.ComponentModel;

namespace CustomClasses.Roles
{
    public class FatClassD
    {
        [Description("O nome do Classe-D gordo")]
        public string FatClassDName { get; set; } = "Classe-D Gordo";

        [Description("A chance de um Classe-D gordo spawnar no incio da partida")]
        public int FatClassDChance { get; set; } = 10;

        [Description("O tanto de vida que um Classe-D gordo terá quando spawnar")]
        public float FatClassDHealth = 200;

        [Description("O tanto de escudo que um Classe-D gordo terá quando spawnar")]
        public float FatClassDShield = 0f;

        [Description("Quantos Classe-D gordo podem spawnar em uma partida")]
        public float FatClassDCount = 2f;

        [Description("A mensagem que aparece quando você spawna de Classe-D gordo")]
        public string FatClassDSpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#FAFB00>ê</color> <color=#F9F900>é</color> <color=#F9F700>u</color><color=#F9F600>m</color> <color=#00EEFF>C</color><color=#00E8FC>l</color><color=#00E2FA>a</color><color=#00DCF8>s</color><color=#00D6F6>s</color><color=#00D1F3>e</color><color=#00CBF1>-</color><color=#00C5EF>D</color> <color=#00B9EB>g</color><color=#00B4E8>o</color><color=#00AEE6>r</color><color=#00A8E4>d</color><color=#00A2E2>o</color><color=#F8F400>,</color> <color=#F8F200>v</color><color=#F8F100>o</color><color=#F7F000>c</color><color=#F7EF00>ê</color> <color=#F7ED00>t</color><color=#F6EC00>e</color><color=#F6EB00>m</color> <color=#F6E900>o</color> <color=#2BFF00>d</color><color=#27F100>o</color><color=#24E400>b</color><color=#21D700>r</color><color=#1DCA00>o</color> <color=#17B000>d</color><color=#13A200>e</color> <color=#0D8800>v</color><color=#097B00>i</color><color=#066E00>d</color><color=#036100>a</color> <color=#F5E600>m</color><color=#F5E500>a</color><color=#F5E400>s</color> <color=#FF0000>-</color><color=#D30000>2</color><color=#A80000>0</color><color=#7D0000>%</color> <color=#F4E100>d</color><color=#F4E000>e</color> <color=#F3DE00>v</color><color=#F3DD00>e</color><color=#F3DC00>l</color><color=#F3DB00>o</color><color=#F2DA00>c</color><color=#F2D900>i</color><color=#F2D800>d</color><color=#F2D700>a</color><color=#F1D600>d</color><color=#F1D500>e</color> <color=#F1D300>d</color><color=#F0D200>e</color> <color=#F0D000>m</color><color=#F0CF00>o</color><color=#F0CE00>v</color><color=#EFCD00>i</color><color=#EFCC00>m</color><color=#EFCB00>e</color><color=#EFCA00>n</color><color=#EEC900>t</color><color=#EEC800>o</color> <color=#EEC600>e</color> <color=#FF0000>-</color><color=#D30000>5</color><color=#A80000>0</color><color=#7D0000>%</color> <color=#EDC300>d</color><color=#EDC200>e</color> <color=#ECC000>s</color><color=#ECBF00>t</color><color=#ECBE00>a</color><color=#ECBD00>m</color><color=#EBBC00>i</color><color=#EBBB00>n</color><color=#EBBA00>a</color><color=#EBB900>.</color>\n";

        [Description("Os items que um Classe-D gordo terá quando spawnar.")]
        public List<string> FatClassDSpawnItems { get; set; } = new()
        {
            "Coin"
        };
    }
}
