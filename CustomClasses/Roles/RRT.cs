using System.Collections.Generic;
using System.ComponentModel;
using CommandSystem.Commands.RemoteAdmin.Inventory;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features;
using MEC;

namespace CustomClasses.Roles
{
    public class RRT
    {
        [Description("O nome da RRT")]
        public string RRTName { get; set; } = "Equipe de Resposta Rápida";

        [Description("A chance de um RRT spawnar no incio da partida")]
        public int RRTChance { get; set; } = 35;

        [Description("O tanto de vida que um RRT terá quando spawnar")]
        public float RRTHealth = 80;

        [Description("O tanto de escudo que um RRT terá quando spawnar")]
        public float RRTShield = 0f;

        [Description("Quantos RRT podem spawnar em uma partida")]
        public float RRTCount = 4f;

        [Description("A mensagem que aparece quando você spawna de RRT")]
        public string RRTSpawnMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFD00>o</color><color=#FAFC00>c</color><color=#FAFB00>ê</color> <color=#F9F900>s</color><color=#F9F800>p</color><color=#F9F700>a</color><color=#F9F600>w</color><color=#F8F500>n</color><color=#F8F400>o</color><color=#F8F300>u</color> <color=#F7F100>c</color><color=#F7F000>o</color><color=#F7EF00>m</color><color=#F7EE00>o</color><color=#F6ED00>:</color> <color=#00EEFF>R</color><color=#00EBFE>R</color><color=#00E8FD>T</color> <color=#00E3FB>(</color><color=#00E0FA>E</color><color=#00DEF9>q</color><color=#00DBF8>u</color><color=#00D9F7>i</color><color=#00D6F6>p</color><color=#00D3F5>e</color> <color=#00CEF3>d</color><color=#00CCF2>e</color> <color=#00C6F0>R</color><color=#00C4EF>e</color><color=#00C1EE>s</color><color=#00BEED>p</color><color=#00BCEC>o</color><color=#00B9EB>s</color><color=#00B7EA>t</color><color=#00B4E9>a</color> <color=#00AFE7>R</color><color=#00ACE6>á</color><color=#00AAE5>p</color><color=#00A7E4>i</color><color=#00A4E3>d</color><color=#00A2E2>a</color><color=#009FE1>)</color><color=#F6EB00>,</color> <color=#F6E900>a</color><color=#F5E800>j</color><color=#F5E600>u</color><color=#F5E500>d</color><color=#F5E400>e</color> <color=#F4E200>o</color><color=#F4E100>s</color> <color=#00EEFF>F</color><color=#00D3F4>T</color><color=#00B8EA>M</color> <color=#F2DB00>a</color> <color=#F2D900>c</color><color=#F2D800>o</color><color=#F2D700>n</color><color=#F1D600>t</color><color=#F1D500>e</color><color=#F1D400>r</color> <color=#F0D200>a</color> <color=#F0D000>s</color><color=#F0CE00>i</color><color=#EFCD00>t</color><color=#EFCC00>u</color><color=#EFCB00>a</color><color=#EFCA00>ç</color><color=#EEC900>ã</color><color=#EEC800>o</color> <color=#EEC600>n</color><color=#EEC500>a</color> <color=#EDC300>i</color><color=#EDC200>n</color><color=#EDC100>s</color><color=#ECC000>t</color><color=#ECBF00>a</color><color=#ECBE00>l</color><color=#ECBD00>a</color><color=#EBBC00>ç</color><color=#EBBB00>ã</color><color=#EBBA00>o</color><color=#EBB900>.</color>\r\n";
        public List<string> RRTSpawnItems { get; set; } = new()
        {
            "GunE11SR",
            "KeycardMTFPrivate",
            "GunCOM18",
            "Radio",
            "Medkit",
            "Painkillers",
            "ArmorCombat",
        };
    };
}
