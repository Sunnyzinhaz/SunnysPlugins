using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features.Items;

namespace CustomClasses.Roles
{
    public class LabRatClassD
    {
        [Description("O nome do Classe-D Rato de Laboratorio")]
        public string LabRatClassDName { get; set; } = "Classe-D Rato de Laboratorio";

        [Description("A chance de um Classe-D Rato de Laboratorio spawnar no incio da partida")]
        public int LabRatClassDChance { get; set; } = 8;

        [Description("O tanto de vida que um Classe-D Rato de Laboratorio terá quando spawnar")]
        public float LabRatClassDHealth = 90;

        [Description("O tanto de escudo que um Classe-D Rato de Laboratorio terá quando spawnar")]
        public float LabRatClassDShield = 0f;

        [Description("Quantos Classe-D Rato de Laboratorio podem spawnar em uma partida")]
        public float LabRatClassDCount = 1f;

        [Description("A mensagem que aparece quando você spawna de Classe-D Rato de Laboratorio")]
        public string LabRatClassDSpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#FAFB00>ê</color> <color=#F9F800>é</color> <color=#F9F600>u</color><color=#F8F500>m</color> <color=#00EEFF>C</color><color=#00EBFD>l</color><color=#00E8FC>a</color><color=#00E5FB>s</color><color=#00E2FA>s</color><color=#00DFF9>e</color><color=#00DCF8>-</color><color=#00D9F7>D</color> <color=#00D3F5>R</color><color=#00D1F3>a</color><color=#00CEF2>t</color><color=#00CBF1>o</color> <color=#00C5EF>d</color><color=#00C2EE>e</color> <color=#00BCEC>L</color><color=#00B9EB>a</color><color=#00B7E9>b</color><color=#00B4E8>o</color><color=#00B1E7>r</color><color=#00AEE6>a</color><color=#00ABE5>t</color><color=#00A8E4>o</color><color=#00A5E3>r</color><color=#00A2E2>i</color><color=#009FE1>o</color>, <color=#F7F000>,</color> <color=#F6ED00>v</color><color=#F6EB00>o</color><color=#F6EA00>c</color><color=#F5E800>ê</color> <color=#F5E500>s</color><color=#F4E400>p</color><color=#F4E200>a</color><color=#F4E100>w</color><color=#F3DF00>n</color><color=#F3DE00>a</color> <color=#F2DB00>n</color><color=#F2DA00>a</color> <color=#F1D700>G</color><color=#F1D500>R</color><color=#F1D400>1</color><color=#F0D200>8</color> <color=#F0CF00>c</color><color=#EFCE00>o</color><color=#EFCC00>m</color> <color=#2BFF00>1</color> <color=#21C500>i</color><color=#1CA800>t</color><color=#178B00>e</color><color=#126E00>m</color> <color=#FA3333>S</color><color=#CB2222>C</color><color=#9C1111>P</color> <color=#EEC600>a</color><color=#EDC500>l</color><color=#EDC300>e</color><color=#EDC200>a</color><color=#ECC000>t</color><color=#ECBF00>ó</color><color=#ECBD00>r</color><color=#EBBC00>i</color><color=#EBBA00>o</color><color=#EBB900>.</color>\n";

        [Description("Os items que um Classe-D Rato de Laboratorio terá quando spawnar.")]
        public List<string> LabRatClassDSpawnItems { get; set; } = new()
        {
            ItemType.SCP018.ToString(),
            ItemType.SCP1576.ToString(),
            ItemType.SCP1853.ToString(),
            ItemType.SCP207.ToString(),
            ItemType.SCP2176.ToString(),
            ItemType.SCP244a.ToString(),
            ItemType.SCP244b.ToString(),
            ItemType.SCP268.ToString(),
            ItemType.SCP330.ToString(),
            ItemType.SCP500.ToString(),
            ItemType.AntiSCP207.ToString(), 
        };
    }
}
