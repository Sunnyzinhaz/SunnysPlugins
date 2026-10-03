using System.Collections.Generic;
using System.ComponentModel;

namespace CustomClasses.Roles
{
    public class Brutus
    {
        [Description("O nome do Brutus")]
        public string BrutusName { get; set; } = "Brutus";

        [Description("A chance de um Brutus spawnar no incio da partida")]
        public int BrutusChance { get; set; } = 0;

        [Description("O tanto de vida que um Brutus terá quando spawnar")]
        public float BrutusHealth = 110;

        [Description("O tanto de escudo que um Brutus terá quando spawnar")]
        public float BrutusShield = 15f;

        [Description("Quantos brutus podem spawnar em uma partida")]
        public float BrutusCount = 1f;

        [Description("A mensagem que aparece quando você spawna de Brutus")]
        public string BrutusSpawnMessage { get; set; } = "<size=55>Você é um <color=red>Brutus</color>, você tem uma vida maior, e pode abrir gates.</size>";

        [Description("Os items que um Brutus terá quando spawnar.")]
        public List<string> BrutusSpawnItems { get; set; } = new List<string>
        {
            "Coin",
            "FlashLight"
        };
    }
}