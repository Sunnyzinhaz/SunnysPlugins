using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features.Items;

namespace CustomClasses.Roles
{
    public class JanitorClassD
    {
        [Description("O nome do Classe-D Faxineiro")]
        public string JanitorClassDName { get; set; } = "Classe-D Faxineiro";

        [Description("A chance de um Classe-D Faxineiro spawnar no incio da partida")]
        public int JanitorClassDChance { get; set; } = 10;

        [Description("O tanto de vida que um Classe-D Faxineiro terá quando spawnar")]
        public float JanitorClassDHealth = 100;

        [Description("O tanto de escudo que um Classe-D Faxineiro terá quando spawnar")]
        public float JanitorClassDShield = 0f;

        [Description("Quantos Classe-D Faxineiro podem spawnar em uma partida")]
        public float JanitorClassDCount = 1f;

        [Description("A mensagem que aparece quando você spawna de Classe-D Faxineiro")]
        public string JanitorClassDSpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#FAFB00>ê</color> <color=#F9F900>é</color> <color=#F9F700>u</color><color=#F8F500>m</color> <color=#00EEFF>C</color><color=#00E9FD>l</color><color=#00E5FB>a</color><color=#00E0F9>s</color><color=#00DCF8>s</color><color=#00D7F6>e</color><color=#00D3F4>-</color><color=#00CEF2>D</color> <color=#00C5EF>F</color><color=#00C1ED>a</color><color=#00BCEC>x</color><color=#00B8EA>i</color><color=#00B3E8>n</color><color=#00AFE6>e</color><color=#00AAE5>i</color><color=#00A6E3>r</color><color=#00A1E1>o</color><color=#F8F300>,</color> <color=#F7F100>v</color><color=#F7F000>o</color><color=#F7EF00>c</color><color=#F7EE00>ê</color> <color=#F6EB00>s</color><color=#F6EA00>p</color><color=#F6E900>a</color><color=#F5E800>w</color><color=#F5E700>n</color><color=#F5E600>a</color> <color=#F4E300>e</color><color=#F4E200>m</color> <color=#F4E000>u</color><color=#F3DF00>m</color> <color=#F3DD00>l</color><color=#F3DC00>u</color><color=#F2DA00>g</color><color=#F2D900>a</color><color=#F2D800>r</color> <color=#F1D600>d</color><color=#F1D500>i</color><color=#F1D400>f</color><color=#F1D300>e</color><color=#F0D100>r</color><color=#F0D000>e</color><color=#F0CF00>n</color><color=#F0CE00>t</color><color=#EFCD00>e</color> <color=#EFCB00>e</color> <color=#EEC800>t</color><color=#EEC700>e</color><color=#EEC600>m</color> <color=#EDC400>m</color><color=#EDC300>a</color><color=#EDC200>i</color><color=#EDC100>s</color> <color=#ECBE00>i</color><color=#ECBD00>t</color><color=#ECBC00>e</color><color=#EBBB00>n</color><color=#EBBA00>s</color><color=#EBB900>.</color>\n";

        [Description("Os items que um Classe-D Faxineiro terá quando spawnar.")]
        public List<string> JanitorClassDSpawnItems { get; set; } = new()
        {
            "Coin",
            "Coin",
            ItemType.KeycardJanitor.ToString()
        };
    }
}
