using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Scp035
{
    public sealed class Config : IConfig
    {
        [Description("Liga/desliga o SCP-035.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Mostra logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("ID do CustomItem SCP-035 definido em CustomItems/Items/Scp035Item.cs.")]
        public uint Scp035ItemId { get; set; } = 99;

        [Description("Configuração da CustomRole SCP-035.")]
        public Scp035Role Scp035RoleConfig { get; set; } = new();

        [Description("Pequeno atraso após o início do uso do SCP-1344 para casar com a animação.")]
        public float PossessionDelay { get; set; } = 0.349999994f;

        [Description("Se true, ao usar o SCP-035 ele transfere o controle para alguém no Spectator. Se false, o próprio jogador que usou a máscara permanece como SCP-035.")]
        public bool PullSpectatorOnPossession { get; set; } = true;

        [Description("Hint exibida quando o jogador pega o SCP-035.")]
        public string PickupHint { get; set; } =
            "<size=28><color=#ff3030><b>Você pegou o SCP-035</b></color></size>\n" +
            "<size=20><color=#ffffff>Ao utilizá-lo, ele possuirá seu corpo.</color></size>";

        [Description("Tempo, em segundos, da hint exibida ao pegar o SCP-035.")]
        public float PickupHintDuration { get; set; } = 0f;

        [Description("Posição vertical da hint de pickup no RueI.")]
        public float PickupHintPosition { get; set; } = 300f;

        [Description("Hint exibida enquanto o SCP-035 estiver selecionado/segurado na mão.")]
        public string HoldingHint { get; set; } =
            "<size=24><color=#ff3030><b>SCP-035</b></color></size>\n" +
            "<size=18><color=#ffffff>Utilize o SCP-1344 para permitir que ele possua seu corpo.</color></size>";

        [Description("Posição vertical da hint enquanto o SCP-035 estiver na mão.")]
        public float HoldingHintPosition { get; set; } = 340f;

        [Description("Intervalo de verificação da hint de item segurado.")]
        public float HoldingHintCheckInterval { get; set; } = 0.5f;

        [Description("Hint exibida para o espectador escolhido quando ele assume o corpo do hospedeiro.")]
        public string PossessedSpectatorHint { get; set; } =
            "<size=24><color=#ff3030><b>SCP-035</b></color></size>\n" +
            "<size=18><color=#ffffff>O jogador <color=#ffd166>{player}</color> colocou a máscara do SCP-035 e você tomou posse de seu corpo...</color></size>";

        [Description("Tempo, em segundos, da hint exibida ao espectador que assume o corpo.")]
        public float PossessedSpectatorHintDuration { get; set; } = 8f;

        [Description("Posição vertical da hint mostrada ao espectador que assume o corpo.")]
        public float PossessedSpectatorHintPosition { get; set; } = 320f;

        [Description("CASSIE quando o SCP-035 assume um hospedeiro.")]
        public string BreachCassie { get; set; } = "pitch_0.95 attention all personnel . SCP 0 3 5 containment breach detected";

        [Description("Mantido por compatibilidade. O anúncio de morte é montado automaticamente para evitar palavras inválidas no CASSIE.")]
        public string DeathCassieTemplate { get; set; } = "bell_start SCP 0 3 5 terminated by {killer} bell_end";

        [Description("Nome exato da schematic do ProjectMER LabAPI.")]
        public string MaskSchematicName { get; set; } = "035";

        [Description("Se true, esconde a máscara apenas do próprio jogador SCP-035. Deixe false para testar/visualizar a máscara normalmente.")]
        public bool HideMaskFromController { get; set; } = false;

        [Description("Atraso antes de prender/sincronizar a máscara depois que o 035 é criado.")]
        public float MaskAttachDelay { get; set; } = 0.5f;

        [Description("Intervalo de sincronização visual da máscara. 0.10 = 10 vezes por segundo.")]
        public float MaskSyncInterval { get; set; } = 0.0160000008f;

        [Description("Offset lateral local da máscara em relação à câmera.")]
        public float MaskLocalX { get; set; } = 0f;

        [Description("Offset vertical local da máscara em relação à câmera.")]
        public float MaskLocalY { get; set; } = -1f;

        [Description("Offset frontal local da máscara em relação à câmera.")]
        public float MaskLocalZ { get; set; } = 0.119999997f;

        [Description("Rotação local X da máscara.")]
        public float MaskLocalPitch { get; set; } = 0f;

        [Description("Rotação local Y da máscara.")]
        public float MaskLocalYaw { get; set; } = 0f;

        [Description("Rotação local Z da máscara.")]
        public float MaskLocalRoll { get; set; } = 0f;

        [Description("TESTE/FALLBACK: prende a máscara no corpo do jogador em vez da câmera.")]
        public bool UseBodyAnchorForMask { get; set; } = true;

        [Description("Altura local da máscara no corpo do jogador.")]
        public float MaskBodyY { get; set; } = -1.20000005f;

        [Description("Distância local da máscara para frente do corpo.")]
        public float MaskBodyZ { get; set; } = 0.200000003f;

        [Description("Intervalo de verificação/log da máscara após o spawn.")]
        public float MaskDebugCheckDelay { get; set; } = 1.5f;

        [Description("Delay, em segundos, após assumir a aparência humana antes de spawnar a máscara. Evita a schematic ser destruída/sumir durante a troca Tutorial -> aparência.")]
        public float MaskSpawnDelaySeconds { get; set; } = 3f;

        [Description("Frequência máxima de sincronização de rede da máscara. O root continua seguindo a cabeça todo frame. Recomendado: 6.")]
        public float MaskNetworkUpdateHz { get; set; } = 60f;

        [Description("LEGADO: não é mais usado. Mantido para não quebrar configs antigas.")]
        public float MaskForwardOffset { get; set; } = 0.119999997f;

        [Description("LEGADO: não é mais usado. Mantido para não quebrar configs antigas.")]
        public float MaskUpOffset { get; set; } = -0.850000024f;

        [Description("Ajuste para frente do ROOT da schematic em relação aos pés do jogador.")]
        public float MaskRootForwardOffset { get; set; } = 0f;

        [Description("Avanço fixo da máscara para fora da face. Novo na v4.15; 0.10 = dez centímetros para frente.")]
        public float MaskFaceForwardOffset { get; set; } = 0.100000001f;

        [Description("Ajuste vertical do ROOT da schematic em relação aos pés do jogador. Normalmente 0.")]
        public float MaskRootUpOffset { get; set; } = -1.20000005f;

        [Description("Ajuste lateral do ROOT da schematic. Positivo = direita, negativo = esquerda.")]
        public float MaskRootRightOffset { get; set; } = 0f;

        [Description("Ajuste local X aplicado depois que a schematic é parentada diretamente na câmera.")]
        public float MaskCameraLocalX { get; set; } = 0f;

        [Description("Ajuste local Y aplicado depois que a schematic é parentada diretamente na câmera.")]
        public float MaskCameraLocalY { get; set; } = 0f;

        [Description("Ajuste local Z aplicado depois que a schematic é parentada diretamente na câmera.")]
        public float MaskCameraLocalZ { get; set; } = 0f;

        [Description("Rotação local extra X (pitch) da schematic depois de parentada na câmera.")]
        public float MaskCameraPitch { get; set; } = 0f;

        [Description("Rotação local extra Y (yaw) da schematic depois de parentada na câmera.")]
        public float MaskCameraYaw { get; set; } = 0f;

        [Description("Rotação local extra Z (roll) da schematic depois de parentada na câmera.")]
        public float MaskCameraRoll { get; set; } = 0f;

        [Description("Altura local do pivô usado para inclinar a schematic junto com o olhar. Ajuste até o pivô coincidir com o centro do rosto/máscara.")]
        public float MaskPitchPivotHeight { get; set; } = 1.54999995f;

        [Description("Deslocamento local para frente do pivô de rotação da máscara.")]
        public float MaskPitchPivotForward { get; set; } = 0f;

        [Description("Permissão necessária para usar o comando 035spawn no Remote Admin.")]
        public string SpawnCommandPermission { get; set; } = "scp035.spawn";

        [Description("Altura adicional do item spawnado pelo comando 035spawn.")]
        public float DebugSpawnYOffset { get; set; } = 0.150000006f;


        [Description("Palavras/nome de CustomItems que o SCP-035 não pode pegar. A checagem ignora cores, tags, espaços e hífens.")]
        public List<string> ForbiddenPickupNames { get; set; } = new()
        {
            "SCP-127",
            "Machete",
            "SCP-1509",
        };


        [Description("O SCP-035 continua ouvindo o chat dos SCPs, mas a própria voz dele é enviada apenas por proximidade.")]
        public bool HearScpChatButSpeakProximity { get; set; } = true;

        [Description("Tipos de item vanilla que o SCP-035 não pode nem iniciar a interação de pickup.")]
        public List<string> ForbiddenVanillaPickupTypes { get; set; } = new()
        {
            "MicroHID",
            "Jailbird",
            "ParticleDisruptor",
        };

        [Description("Impede dano apenas entre o SCP-035 e outros SCPs. Humanos continuam podendo causar/receber dano normalmente.")]
        public bool BlockAlliedFriendlyFire { get; set; } = true;

        [Description("Impede que o SCP-035 seja adicionado como alvo do SCP-096.")]
        public bool IgnoreScp096Trigger { get; set; } = true;

        [Description("Impede que o SCP-035 conte como observador do SCP-173.")]
        public bool IgnoreScp173Observation { get; set; } = true;

        [Description("Escreve no console onde o SCP-035 spawnou no início da rodada.")]
        public bool LogNaturalSpawnLocation { get; set; } = true;

        [Description("Permite usar dummies/bots que estejam em Spectator como controller do SCP-035.")]
        public bool AllowSpectatorDummies { get; set; } = true;
    }
}
