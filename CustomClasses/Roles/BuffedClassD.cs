using System.Collections.Generic;
using System.ComponentModel;

namespace CustomClasses.Roles
{
    public class BuffedClassD
    {
        [Description("O nome do Classe-D bombado")]
        public string BuffedClassDName { get; set; } = "Classe-D Bombado";

        [Description("A chance de um Classe-D Bombado spawnar no incio da partida")]
        public int BuffedClassDChance { get; set; } = 10;

        [Description("O tanto de vida que um Classe-D bombado terá quando spawnar")]
        public float BuffedClassDHealth = 150;

        [Description("O tanto de escudo que um Classe-D Bombado terá quando spawnar")]
        public float BuffedClassDShield = 0f;

        [Description("Quantos Classe-D Bombado podem spawnar em uma partida")]
        public float BuffedClassDCount = 2f;

        [Description("A mensagem que aparece quando você spawna de Classe-D Bombado")]
        public string BuffedClassDSpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFC00>o</color><color=#F9FA00>c</color><color=#F9F700>ê</color> <color=#F8F300>é</color> <color=#F7EE00>u</color><color=#F6EC00>m</color> <color=#00EEFF>C</color><color=#00E8FD>l</color><color=#00E3FB>a</color><color=#00DEF9>s</color><color=#00D9F7>s</color><color=#00D4F5>e</color><color=#00CFF3>-</color><color=#00CAF1>D</color> <color=#00C0ED>B</color><color=#00BBEB>o</color><color=#00B6E9>m</color><color=#00B1E7>b</color><color=#00ACE5>a</color><color=#00A7E3>d</color><color=#00A2E1>o</color><color=#F5E700>,</color> <color=#F4E200>v</color><color=#F4E000>o</color><color=#F3DD00>c</color><color=#F3DB00>ê</color> <color=#F1D600>t</color><color=#F1D400>e</color><color=#F0D200>m</color> <color=#2BFF00>1</color><color=#27E900>5</color><color=#23D300>0</color> <color=#EFCA00>d</color><color=#EEC800>e</color> <color=#1EB200>v</color><color=#1A9E00>i</color><color=#178B00>d</color><color=#147800>a</color><color=#116500>.</color>\n";

        [Description("Os items que um Classe-D Bombado terá quando spawnar.")]
        public List<string> BuffedClassDSpawnItems { get; set; } = new()
        {
            "Coin"
        };
    }
}
