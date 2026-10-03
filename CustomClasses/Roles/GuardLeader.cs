using Exiled.API.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace CustomClasses.Roles
{
    public class GuardLeader
    {
        [Description("O tanto de vida que um Guarda Lider terá quando spawnar")]
        public float GuardLeaderHealth = 105f;

        [Description("O tanto de escudo que um Guara Lider terá quando spawnar")]
        public float GuardLeaderShield = 0f;

        [Description("O nome do Guarda Lider")]
        public string GuardLeaderName { get; set; } = "GuardLeader";

        [Description("A chance de um Guarda Lider spawnar no incio da partida")]
        public int GuardLeaderChance { get; set; } = 20;

        [Description("Quantos Guardas Líder podem spawnar em uma partida")]
        public float LeaderCount = 1f;

        [Description("A mensagem que aparece quando você spawna como um guarda lider")]
        public string GuardLeaderMessage { get; set; } = "<color=#FBFF00>V</color><color=#FAFC00>o</color><color=#F9FA00>c</color><color=#F9F700>ê</color> <color=#F8F200>f</color><color=#F7F000>o</color><color=#F7ED00>i</color> <color=#F6E800>d</color><color=#F5E600>e</color><color=#F4E400>s</color><color=#F4E100>i</color><color=#F3DF00>g</color><color=#F3DC00>n</color><color=#F2DA00>a</color><color=#F2D700>d</color><color=#F1D500>o</color> <color=#F0D000>c</color><color=#EFCE00>o</color><color=#EFCB00>m</color><color=#EEC900>o</color> <color=#00EEFF>L</color><color=#00E9FD>í</color><color=#00E5FB>d</color><color=#00E0F9>e</color><color=#00DCF8>r</color> <color=#00D3F4>d</color><color=#00CEF2>e</color> <color=#00C5EF>E</color><color=#00C1ED>s</color><color=#00BCEC>q</color><color=#00B8EA>u</color><color=#00B3E8>a</color><color=#00AFE6>d</color><color=#00AAE5>r</color><color=#00A6E3>ã</color><color=#00A1E1>o</color> <color=#EDC100>d</color><color=#ECBF00>a</color> <color=#A5ABAC>G</color><color=#A0A6A7>u</color><color=#9CA1A2>a</color><color=#979D9D>r</color><color=#939899>d</color><color=#8F9494>a</color> <color=#868A8A>d</color><color=#818686>a</color> <color=#797D7C>I</color><color=#747878>n</color><color=#707473>s</color><color=#6C6F6E>t</color><color=#676A69>a</color><color=#636665>l</color><color=#5E6160>a</color><color=#5A5D5B>ç</color><color=#565857>ã</color><color=#515452>o</color><color=#4D4F4D>.</color>\r\n";

        public List<string> GuardLeaderSpawnItems { get; set; } = new()
        {
            "GunCrossvec",
            "KeycardMTFPrivate",
            "Radio",
            "Medkit",
            "ArmorCombat"
        };
        public void GiveRRTItems(Player player, GuardLeader config)
        {
            foreach (var itemName in config.GuardLeaderSpawnItems)
            {
                if (System.Enum.TryParse(itemName, out ItemType type))
                    player.AddItem(type);
            }

            // Exemplo: Dá 100 munições de NATO556
            player.AddAmmo(AmmoType.Nato9, 100);
        }

    }
}
