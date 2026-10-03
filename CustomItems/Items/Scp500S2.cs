using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomItems.Core;
using CustomItems.Core.Extensions;
using CustomItems.Core.Interfaces;
using CustomItems.Utils;
using DG.Tweening;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

using Random = UnityEngine.Random;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500S2 : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        public readonly static (Predicate<float> predicate, string message)[] Messages = [
            (value => value >= 1.1, "muito alto"),
            (value => value > 1, "um pouco alto"),
            (value => value == 1, "normal"),
            (value => value <= 0.8, "muito baixo"),
            (value => value < 1, "um pouco baixo"),
        ];

        public readonly (EffectType EffectType, byte Uppedintensity, float EffectDuration, float Duration)[] TransitionEffects = [
            (EffectType: EffectType.Slowness, Uppedintensity: 20, EffectDuration: 0f, Duration: 5f),
            (EffectType: EffectType.Blurred, Uppedintensity: 1, EffectDuration: 1f, Duration: 0f),
        ];

        // Para verificação e gerenciamento de timers
        public readonly HashSet<Player> SizedPlayers = [];
        public readonly Dictionary<Player, TimeSpan> PlayerExpirationTimes = [];

        [Description("Mensagem que aparece quando o jogador é um SCP.")]
        public readonly string BroadcastIsScp035 = "\n<size=30>Você é um SCP, portanto não sofreu mudanças do uso deste item.</size>";
        public readonly string BroadcastAlreadySized = "\n<size=30>Você ingere as pílulas novamente...\nVocê sente que o efeito foi prolongado...</size>";
        public readonly string BroadcastBackToNormal = "\n<size=30>Você se sente que esta voltando ao normal...</size>";
        
        [Description("Mensagem que aparece quando o jogador esta alterando de tamanho.")]
        public readonly string BroadcastTimeScale = "\n<size=30>Você se sente... {0}...?</size>";

        [Description("Mensagem do hint que aparece na tela do player enquanto o tempo do efeito está rolando.")]
        public readonly string HintMessageHud = "<size=20>💊 Estabilidade do <color=red>SCP-500-S-2</color></size>\n<line-height=0%>{1}</size>\n<size=25><b><voffset=3>{0}</voffset></b></size></line-height>";
        public readonly float YHintPosition = 100f;

        [Description("Informação que é enviado no console do player quando utilizado o SCP-500-S-2.")]
        public readonly string ConsoleInfoMessage = "Você ficou sobre efeitos do SCP-500-S-2... Mas o efeito não é permanente. O efeito passará após um certo tempo.";

        [Description("Faixa de tamanho do jogador.")]
        public readonly (float MinScale, float MaxScale) ScaleRange = (0.7f, 1.2f);

        [Description("Tempo de reversão do tamanho do jogador (em segundos).")]
        public readonly float TimeReversal = 120f;
        [Description("Tempo de atualização da coroutine (checagem) (em segundos).")]
        public readonly float TimeCoroutineUpdate = 1f;

        public override uint Id { get; set; } = 6;
        public override string Name { get; set; } = "SCP-500-S-2";
        public override string Description { get; set; } =
            "<color=#FBFF00>A</color><color=#FAFC00>l</color><color=#F9F900>t</color><color=#F9F600>e</color><color=#F8F300>r</color><color=#F7F000>a</color> <color=#F6EB00>s</color><color=#F5E800>e</color><color=#F5E500>u</color> <color=#00EEFF>t</color><color=#00E2FA>a</color><color=#00D6F6>m</color><color=#00CBF1>a</color><color=#00BFED>n</color><color=#00B4E8>h</color><color=#00A8E4>o</color> <color=#F3DF00>a</color><color=#F3DC00>l</color><color=#F2DA00>e</color><color=#F2D700>a</color><color=#F1D400>t</color><color=#F0D100>o</color><color=#F0CE00>r</color><color=#EFCB00>i</color><color=#EEC900>a</color><color=#EEC600>m</color><color=#EDC300>e</color><color=#ECC000>n</color><color=#ECBD00>t</color><color=#EBBA00>e</color><color=#EBBA00>.</color>\n";
        public override float Weight { get; set; } = 1f;

        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-S-2</b></color></size>\n" +
            "<size=18><color=#FBFF00>A</color><color=#FAFC00>l</color><color=#F9F900>t</color><color=#F9F600>e</color><color=#F8F300>r</color><color=#F7F000>a</color> <color=#F6EB00>s</color><color=#F5E800>e</color><color=#F5E500>u</color> <color=#00EEFF>t</color><color=#00E2FA>a</color><color=#00D6F6>m</color><color=#00CBF1>a</color><color=#00BFED>n</color><color=#00B4E8>h</color><color=#00A8E4>o</color> <color=#F3DF00>a</color><color=#F3DC00>l</color><color=#F2DA00>e</color><color=#F2D700>a</color><color=#F1D400>t</color><color=#F0D100>o</color><color=#F0CE00>r</color><color=#EFCB00>i</color><color=#EEC900>a</color><color=#EEC600>m</color><color=#EDC300>e</color><color=#ECC000>n</color><color=#ECBD00>t</color><color=#EBBA00>e</color><color=#EBBA00>.</color></size>";

        // Remove as mensagens automáticas do Exiled.CustomItems.
        // O CustomHeldHintService cuida da interface visual.
        protected override void ShowPickedUpMessage(Exiled.API.Features.Player player) { }
        protected override void ShowSelectedMessage(Exiled.API.Features.Player player) { }

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            // O spawn natural é controlado pelo ServerHandler.
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(),
            LockerSpawnPoints = new List<LockerSpawnPoint>(),
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = Color.yellow;
        public float GlowRange { get; set; } = 0.2f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } =  ICustomItemGlow.GlowShadowType.Soft;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.Died += OnDeath;
            Exiled.Events.Handlers.Player.CancellingItemUse += OnCancellingItemUse;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Player.Died -= OnDeath;
            Exiled.Events.Handlers.Player.CancellingItemUse -= OnCancellingItemUse;
            base.UnsubscribeEvents();
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (!SizedPlayers.Contains(ev.Player)) return;

            ev.Player.Scale = Vector3.one;
            SizedPlayers.Remove(ev.Player);

            ev.Player.ShowHint(BroadcastBackToNormal, 5f);
        }

        private void OnDeath(DiedEventArgs ev)
        {
            if (!SizedPlayers.Contains(ev.Player)) return;

            ev.Player.Scale = Vector3.one;
            SizedPlayers.Remove(ev.Player);
        }
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            // Marca o item como sendo cancelado
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;  // Impede o uso do item
            }
        }

        //Changes player's height on using item
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            if (!CustomItemUseGate.TryBegin(ev.Item))
                return;

            if (Scp035Check.IsScp035(ev.Player))
            {
                ev.Player.ShowHint(BroadcastIsScp035, 5f);
                Timing.CallDelayed(0.2f, ev.Item.Destroy);
                return;
            }
            else if (PlayerExpirationTimes.ContainsKey(ev.Player))
            {
                // Reseta o tempo para o tempo de expiração (2 min)
                //PlayerExpirationTimes[ev.Player] = Round.ElapsedTime.Add(TimeSpan.FromSeconds(TimeReversal));
                // Adiciona tempo extra de expiração (+2 min)
                var newTime = PlayerExpirationTimes[ev.Player];
                PlayerExpirationTimes[ev.Player] = newTime.Add(TimeSpan.FromSeconds(TimeReversal));

                ev.Player.ShowHint(BroadcastAlreadySized, 5f);
                Timing.CallDelayed(1f, ev.Item.Destroy);
                return;
            }

            var randomScale = Random.Range(ScaleRange.MinScale, ScaleRange.MaxScale);
            var randomScaleVector = new Vector3(randomScale, randomScale, randomScale);
            var message = Messages.First(m => m.predicate(randomScale)).message;

            Log.Debug($"[500S2] Scale factor: {randomScale}, Random Scale Vector: {randomScaleVector}, Message: {message}");

            var transitionsEffects = TransitionEffects
                .Select(t => (ev.Player.RememberEffect(t.EffectType), t.Uppedintensity, t.EffectDuration, t.Duration))
                .ToArray();
            
            foreach (var (effect, uppedintensity, effectDuration, duration) in transitionsEffects)
            {
                var newIntensity = Convert.ToByte(Math.Min(effect.Intensity + uppedintensity, byte.MaxValue));
                Tweens.TweenEffect(ev.Player, effect.Type, effect.Intensity, newIntensity, effectDuration, duration, Ease.InOutQuad);
            }

            Tweens.TweenScale(ev.Player, ev.Player.Scale, randomScaleVector, 10f, Ease.OutBounce, () => 
            {
                foreach (var (effect, _, _, _) in transitionsEffects)
                {
                    effect.Restore();
                }
            });

            SizedPlayers.Add(ev.Player);

            ev.Player.ShowHint(string.Format(BroadcastTimeScale, message), 5f);
            ev.Player.SendConsoleMessage(ConsoleInfoMessage, "cyan");

            var expirationTime = Round.ElapsedTime.Add(TimeSpan.FromSeconds(TimeReversal));
            PlayerExpirationTimes[ev.Player] = expirationTime;
            var playerCoroutine = Timing.RunCoroutine(ITimeScale(ev.Player), ev.Player.GameObject);
            
            Timing.CallDelayed(1f, ev.Item.Destroy);
        }

        private IEnumerator<float> ITimeScale(Player player)
        {
            Log.Debug($"[500S2] Player: {player.Nickname} is starting coroutine.");
            bool runningGood = true;

            int progressBarLength = 12;
            var progressBar = new BarGenerator(progressBarLength);

            while (runningGood && Round.ElapsedTime < PlayerExpirationTimes[player])
            {
                runningGood = ValidateCoroutine(player);

                var timeRemaining = TimeSpan.FromSeconds(Math.Max(0, (PlayerExpirationTimes[player] - Round.ElapsedTime).TotalSeconds));
                int progressBarFill = (int)Math.Round(timeRemaining.TotalSeconds / TimeReversal * progressBarLength);

                var textProgressBar = progressBar.Update(progressBarFill);
                var textTimeRemaining = $"{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

                player.ShowHint(string.Format(HintMessageHud, textTimeRemaining, textProgressBar), TimeCoroutineUpdate);

                yield return Timing.WaitForSeconds(TimeCoroutineUpdate);
            }
            
            if (!runningGood)
            {
                Log.Debug($"[500S2] Player: {player?.Nickname} is null, dead or not in SizedPlayers list. Ending coroutine.");
                
                if (player is null) yield break;

                SizedPlayers.Remove(player);
                PlayerExpirationTimes.Remove(player);

                player.Scale = Vector3.one;
                player.SendConsoleMessage($"Seu tamanho foi redefinido para o padrão. {Vector3.one}", "cyan");

                yield break;
            }
            
            RevertScale(player);
        }

        public void RevertScale(Player player, bool listVerification = true)
        {
            if (listVerification && !SizedPlayers.Contains(player)) return;
            Log.Debug($"[500S2] Player: {player.Nickname} is reverting scale.");

            player.ShowHint("\n<size=30>Você sente que está voltando ao normal...</size>", 5f);
            Tweens.TweenScale(player, player.Scale, Vector3.one, 10f, Ease.OutBounce);

            SizedPlayers.Remove(player);
            PlayerExpirationTimes.Remove(player);
        }

        public bool ValidateCoroutine(Player player)
        {
            return player is not null && player.IsAlive && SizedPlayers.Contains(player) && PlayerExpirationTimes.ContainsKey(player);
        }
    }
}
