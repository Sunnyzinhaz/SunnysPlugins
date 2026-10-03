using System.ComponentModel;

namespace Scp035
{
    // Apenas configurações do SCP-035. Não é mais registrada como CustomRole.
    // Isso evita a mensagem branca de spawn e, principalmente, evita que o
    // EXILED tente trocar/travar o Role real do jogador possuído.
    public sealed class Scp035Role
    {
        [Description("Vida máxima do SCP-035.")]
        public int MaxHealth { get; set; } = 500;

        [Description("Dano por tick da corrosão permanente.")]
        public float CorrosionDamage { get; set; } = 4f;

        [Description("Intervalo, em segundos, entre os ticks de corrosão.")]
        public float CorrosionInterval { get; set; } = 2f;

        [Description("Mantido por compatibilidade com configs antigas.")]
        public bool KeepInventoryOnSpawn { get; set; } = true;

        [Description("Mantido por compatibilidade com configs antigas.")]
        public bool KeepPositionOnSpawn { get; set; } = true;
    }
}
