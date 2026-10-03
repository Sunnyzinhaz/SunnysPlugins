using CustomItems.Utils;
using CustomItems.Core;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomItems.Core.Interfaces;
using EventsPlayer = Exiled.Events.Handlers.Player;
using UnityEngine;

namespace CustomItems.Items
{
    public class RevertCoroutine
    {
        readonly Player Player;
        readonly GameObject GameObject;
        float TimeToRevert { get; set; }
        Action OnFinish { get; init; }
        TimeSpan LastUse { get; set; }
        private CoroutineHandle _coroutineHandle;
        CoroutineHandle Coroutine
        {
            get => _coroutineHandle;
            set
            {
                if (_coroutineHandle != null)
                    Timing.KillCoroutines(_coroutineHandle);
                _coroutineHandle = value;
            }
        }

        public RevertCoroutine(Player player, float timeToRevert, Action onFinish = null)
        {
            GameObject = player.GameObject;
            OnFinish = onFinish;
            TimeToRevert = timeToRevert;
            Coroutine = GenerateCoroutine();
        }

        public void ResetCoroutine()
        {
            Log.Debug($"Resetting revert coroutine for {Player.Nickname}");

            Coroutine = GenerateCoroutine();
        }

        public void ResetCoroutine(float newTime)
        {
            TimeToRevert = newTime;
            ResetCoroutine();
        }

        public void Stop()
        {
            Log.Debug($"Stopping revert coroutine for {Player?.Nickname ?? "Unknown Player"}");

            Timing.KillCoroutines(_coroutineHandle);
        }

        private CoroutineHandle GenerateCoroutine()
        {
            Log.Debug($"Generating revert coroutine for {Player.Nickname} with time to revert: {TimeToRevert}");

            return Timing.CallDelayed(TimeToRevert, () =>
            {
                Log.Debug($"Revert coroutine finished for {Player.Nickname}. Applying revert effects.");
                OnFinish?.Invoke();
            }, GameObject);
        }

        //public void Finish()
        //{

        //    byte playerBaseMovementBoost = Player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0;
        //    byte to = (byte)Math.Clamp(playerBaseMovementBoost - EnergyExtraValue, 0, byte.MaxValue);

        //    Tweens.TweenEffect(Player, EffectType.MovementBoost, playerBaseMovementBoost, to, 2f, 5f);

        //}
    }
    record struct PlayerState(RevertCoroutine RevertCoroutine, float SpeedMultiplier, float SpeedExtraValue, string EnergyString);

    [CustomItem(ItemType.SCP500)]
    public class Scp500S1 : CustomItem, ICustomItemGlow, ICustomItemHeldHint
    {
        public override uint Id { get; set; } = 5;
        public override string Name { get; set; } = "SCP-500-S-1";
        public override string Description { get; set; } =
            "<color=#FBFF00>A</color><color=#FAFC00>l</color><color=#F9F900>t</color><color=#F9F700>e</color><color=#F8F400>r</color><color=#F8F100>a</color> <color=#F6EC00>s</color><color=#F6E900>u</color><color=#F5E700>a</color> <color=#00EEFF>v</color><color=#00E5FB>e</color><color=#00DDF8>l</color><color=#00D5F5>o</color><color=#00CDF2>c</color><color=#00C5EF>i</color><color=#00BDEC>d</color><color=#00B5E9>a</color><color=#00ADE6>d</color><color=#00A5E3>e</color> <color=#F3DF00>a</color><color=#F3DC00>l</color><color=#F2DA00>e</color><color=#F2D700>a</color><color=#F1D400>t</color><color=#F0D200>o</color><color=#F0CF00>r</color><color=#EFCD00>i</color><color=#EFCA00>a</color><color=#EEC700>m</color><color=#EDC500>e</color><color=#EDC200>n</color><color=#ECBF00>t</color><color=#ECBD00>e</color><color=#EBBA00>.</color>";
        public override float Weight { get; set; } = 1f;

        [Description("Mensagem que aparece quando o jogador é um SCP.")]
        public string BroadcastIsScp035 = "\n<size=30>Você é um SCP, portanto não sofreu mudanças do uso deste item</size>";

        [Description("Todas as versões extensa do valor multiplicador de energia que aparece nas mensagens na tela do jogador.")]
        public string[] EnergyStringsUpper =
        [
            "pequeno volume de energia extra",
            "volume de energia extra",
            "grande volume de energia extra",   
        ];
        public string[] EnergyStringsDown =
        [
            "leve peso de energia pesada",
            "peso de energia negativa",
            "grande peso de energia negativa",
        ];
        public string FallbackEnergyString = "sentimento de energia";


        [Description("Mensagem que aparece quando o jogador utiliza a pílula. Essa mensagem não aparece se o valor aleatório não for igual a 1 (nenhuma alteração). {0} é uma definição extensa da energia.")]
        public string BroadcastEffectApplied = "\n<size=30>Ao tomar a pílula, você sente um {0} espalhando por seu corpo</size>";
        public string BroadcastEffectTimerUp = "\n<size=30>Você sente que ao tomar a pílula, você sente que essa energia teve sua força aumentada</size>";

        [Description("Mensagem que aparece quando o efeito do item é removido ou terminado. {0} é uma definição extensa da energia.")]
        public string BroadcastEffectRemoved = "\n<size=30>O {0} desvaneceu em você durante sua morte</size>";
        public string BroadcastEffectTimeout = "\n<size=30>O {0} parece ter finalmente se esgotado</size>";
        public string BroadcastEffectNeutral = "\n<size=30>Ao tomar a pílula você sente... Que nada mudou...\nSera que era pra algo acontecer...?</size>";

        [Description("Faixa de velocidade do jogador.")]
        public (float MinSpeed, float MaxSpeed) SpeedMultiplyRange = (0.5f, 2f);

        [Description("Tempo de reversão da velocidade do jogador (em segundos).")]
        public float TimeReversal = 15f;
        public float DurationTween = 5f;
        public float BaseMaxVelocity { get; set; } = 6f;
        public float EffectMultiplier { get; set; } = 10f;

        private Dictionary<Player, PlayerState> _trackedPlayers = [];

        private static readonly SpawnLocationType[] _spawnLocations =
        [
            SpawnLocationType.InsideLczWc,
            SpawnLocationType.InsideLczCafe,
            SpawnLocationType.Inside049Armory,
            SpawnLocationType.Inside079First,
            SpawnLocationType.Inside079Secondary,
            SpawnLocationType.Inside096,
            SpawnLocationType.Inside173Armory,
            SpawnLocationType.Inside173Bottom,
            SpawnLocationType.Inside173Connector,
            SpawnLocationType.Inside173Gate,
            SpawnLocationType.Inside330,
            SpawnLocationType.Inside330Chamber,
            SpawnLocationType.Inside914,
            SpawnLocationType.InsideEscapePrimary,
            SpawnLocationType.InsideEscapeSecondary,
            SpawnLocationType.InsideGateA,
            SpawnLocationType.InsideGateB,
            SpawnLocationType.InsideGr18,
            SpawnLocationType.InsideHczArmory,
            SpawnLocationType.InsideIntercom,
            SpawnLocationType.InsideLczArmory,
            SpawnLocationType.InsideSurfaceNuke
        ];

        // Hint limpa exibida enquanto esta SCP-500 custom estiver selecionada.
        public bool HasCustomHeldHint { get; set; } = true;
        public float HeldHintPosition { get; set; } = 340f;
        public string HeldHint { get; set; } =
            "<size=28><color=#ff3030><b>SCP-500-S-1</b></color></size>\n" +
            "<size=18><color=#FBFF00>A</color><color=#FAFC00>l</color><color=#F9F900>t</color><color=#F9F700>e</color><color=#F8F400>r</color><color=#F8F100>a</color> <color=#F6EC00>s</color><color=#F6E900>u</color><color=#F5E700>a</color> <color=#00EEFF>v</color><color=#00E5FB>e</color><color=#00DDF8>l</color><color=#00D5F5>o</color><color=#00CDF2>c</color><color=#00C5EF>i</color><color=#00BDEC>d</color><color=#00B5E9>a</color><color=#00ADE6>d</color><color=#00A5E3>e</color> <color=#F3DF00>a</color><color=#F3DC00>l</color><color=#F2DA00>e</color><color=#F2D700>a</color><color=#F1D400>t</color><color=#F0D200>o</color><color=#F0CF00>r</color><color=#EFCD00>i</color><color=#EFCA00>a</color><color=#EEC700>m</color><color=#EDC500>e</color><color=#EDC200>n</color><color=#ECBF00>t</color><color=#ECBD00>e</color><color=#EBBA00>.</color></size>";

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
            EventsPlayer.UsingItem += OnUsingItem;
            EventsPlayer.CancellingItemUse += OnCancellingItemUse;

            EventsPlayer.Died += OnPlayerDeath;
            EventsPlayer.Left += OnPlayerLeft;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            EventsPlayer.UsingItem -= OnUsingItem;
            EventsPlayer.CancellingItemUse -= OnCancellingItemUse;

            EventsPlayer.Died -= OnPlayerDeath;
            EventsPlayer.Left -= OnPlayerLeft;

            base.UnsubscribeEvents();
        }

        // Cancela o método de impedir o jogador parar de utilizar o item, impedindo que ele receba os efeitos do item mesmo apos cancelar o uso.
        private void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;
            }
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            if (_trackedPlayers.Remove(ev.Player))
            {
                Log.Debug($"Player left: {ev.Player.Nickname} tracked list with {_trackedPlayers.Count} remaining.");
            }
        }

        private void OnPlayerDeath(DiedEventArgs ev)
        {
            if (!_trackedPlayers.TryGetValue(ev.Player, out var playerInfo))
            {
                return;
            }

            playerInfo.RevertCoroutine.Stop();
            _trackedPlayers.Remove(ev.Player);

            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 0);
            ev.Player.ShowHint(string.Format(BroadcastEffectRemoved, GetEnergyString(playerInfo.SpeedMultiplier)), 5f);
        }

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

            Log.Debug("Player usou SCP-500-S-1");

            Timing.CallDelayed(1.3f, () =>
            {
                Log.Debug("Aplicando efeito do item");
                Execute(ev.Player);
                ev.Item.Destroy();
            }, ev.Player.GameObject);
        }

        private void Execute(Player player)
        {
            if (_trackedPlayers.ContainsKey(player))
            {
                var playerState = _trackedPlayers[player];
                player.ShowHint(string.Format(BroadcastEffectTimerUp, playerState.EnergyString), 5f);
                playerState.RevertCoroutine.ResetCoroutine();
                return;
            }

            float energyMultiplier = UnityEngine.Random.Range(SpeedMultiplyRange.MinSpeed, SpeedMultiplyRange.MaxSpeed);
            if (Aprox(energyMultiplier, 1f, 0.05f))
            {
                player.ShowHint(BroadcastEffectNeutral, 5f);
                return;
            }

            Log.Debug($"Energy multiplier: {energyMultiplier}");

            float playerBaseVelocity = GetPlayerVelocity(player);
            float energyExtraValue = Math.Abs((playerBaseVelocity * energyMultiplier) - playerBaseVelocity) * EffectMultiplier;

            Log.Debug($"Energy extra value: {energyExtraValue}");

            string energyString = GetEnergyString(energyMultiplier);
            player.ShowHint(string.Format(BroadcastEffectApplied, energyString), 5f);

            Log.Debug($"Applying cutscene effects to {player.Nickname} / Initial Speed: {playerBaseVelocity}, Energy Extra Value: {energyExtraValue}");

            EffectType changedEffect = SetEffectDynamic(player, energyExtraValue, energyMultiplier < 1);
            Log.Debug($"Effect is {changedEffect}");

            player.ChangeEffectIntensity(EffectType.Invigorated, 1, 5);
            //Log.Debug("After applying invigorated effect");

            var coroutine = new RevertCoroutine(player, TimeReversal, () =>
            {
                Log.Debug($"Revert coroutine finished for {player.Nickname}. Effect: {changedEffect}. Removing from list.");

                player.ShowHint(string.Format(BroadcastEffectTimeout, energyString), 5f);
                player.ChangeEffectIntensity(changedEffect, 0, 0);

                _trackedPlayers.Remove(player);
            });

            Log.Debug("Starting revert coroutine");
            _trackedPlayers[player] = new PlayerState(coroutine, energyMultiplier, energyExtraValue, energyString);
        }

        private EffectType SetEffectDynamic(Player player, float extraValue, bool isSlow)
        {
            EffectType effectType = isSlow ? EffectType.Slowness : EffectType.MovementBoost;

            byte OriginalEffectIntensity = player.GetEffect(effectType)?.Intensity ?? 0;
            Tweens.TweenEffect(player, effectType, OriginalEffectIntensity, ToByte(OriginalEffectIntensity + extraValue), TimeReversal, DurationTween);

            return effectType;
        }

        private float GetPlayerVelocity(Player player)
        {
            byte movementBoost = player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0;
            byte scp207Boost = player.GetEffect(EffectType.Scp207)?.Intensity ?? 0;
            return Math.Max((movementBoost + scp207Boost) * BaseMaxVelocity, BaseMaxVelocity);
        }

        public static byte ToByte(float value)
        {
            return (byte)Math.Round(Math.Min(value, byte.MaxValue));
        }
        public static byte ToByte(int value)
        {
            return (byte)Math.Min(value, byte.MaxValue);
        }

        public static bool Aprox(float value, float target, float tolerance)
        {
            return Math.Abs(value - target) <= Math.Abs(target * tolerance);
        }

        private string GetEnergyString(float multiplier)
        {
            float normalized;
            string[] listString;

            if (multiplier < 1)
            {
                float range = 1f - SpeedMultiplyRange.MinSpeed;
                normalized = range == 0 ? 0 : (multiplier - SpeedMultiplyRange.MinSpeed) / range;
                listString = EnergyStringsDown;
            }
            else
            {
                float range = SpeedMultiplyRange.MaxSpeed - 1f;
                normalized = range == 0 ? 0 : (multiplier - 1f) / range;
                listString = EnergyStringsUpper;
            }

            normalized = Mathf.Clamp(normalized, 0f, 1f);

            int index = (int)(normalized * listString.Length);
            if (index == listString.Length)
                index--;

            return listString[index];
        }
    }
}