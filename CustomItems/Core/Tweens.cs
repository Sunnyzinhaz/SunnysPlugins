using DG.Tweening;
using Exiled.API.Features;
using Exiled.API.Enums;
using System;
using UnityEngine;

namespace CustomItems.Core;

public static class Tweens
{
    /// <summary>
    /// Aplica uma transição suave (Tween) na intensidade de um efeito visual em um jogador, interpolando o valor de <paramref name="from"/> até <paramref name="to"/> durante o tempo especificado.
    /// </summary>
    /// <param name="player">O jogador que receberá o efeito.</param>
    /// <param name="effect">O tipo de efeito a ser aplicado.</param>
    /// <param name="from">Valor inicial da intensidade do efeito.</param>
    /// <param name="to">Valor final da intensidade do efeito.</param>
    /// <param name="effectDuration">Duração total do efeito após o Tween (0 para não aplicar duração extra).</param>
    /// <param name="duration">Duração da transição Tween em segundos.</param>
    /// <param name="ease">A função de easing a ser usada.</param>
    public static Tweener TweenEffect(Player player, EffectType effect, byte from, byte to, float effectDuration, float duration, Ease ease = Ease.InOutQuad, Action onComplete = null)
    {
        return OtimizedTween((val) => player.ChangeEffectIntensity(effect, (byte)val, 0f), () =>
        {
            if (effectDuration > 0f)
                player.ChangeEffectIntensity(effect, to, effectDuration);

            onComplete?.Invoke();
        }, from, to, duration, ease);
    }

    public static Tweener TweenEffect(Player player, EffectType effect, byte from, byte to, float effectDuration, float duration, Action onComplete)
    {
        return TweenEffect(player, effect, from, to, effectDuration, duration, Ease.InOutQuad, onComplete);
    }

    public static Tweener TweenScale(Player player, Vector3 from, Vector3 to, float duration, Ease ease = Ease.InOutQuad, Action onComplete = null)
    {
        return OtimizedTween((val) => player.Scale = val, onComplete, from, to, duration, ease);
    }

    //public static Tweener Tween(this Vector3 from, Vector3 to, float duration, Ease ease = Ease.InOutQuad)
    //{
    //    return OtimizedTween((val) => from = val, from, to, duration, ease);
    //}

    public static Tweener OtimizedTween(Action<float> action, Action onComplete, float from, float to, float duration, Ease ease = Ease.InOutQuad)
    {
        float lastVal = from;
        return DOVirtual.Float(from, to, duration, (val) =>
        {
            if (val == lastVal) return;
            lastVal = val;
            action(val);
        }).SetEase(ease).OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener OtimizedTween(Action<float> action, float from, float to, float duration, Ease ease = Ease.InOutQuad)
    {
        return OtimizedTween(action, null, from, to, duration, ease);
    }

    public static Tweener OtimizedTween(Action<int> action, Action onComplete, int from, int to, float duration, Ease ease = Ease.InOutQuad)
    {
        int lastVal = from;
        
        return DOVirtual.Int(from, to, duration, (val) =>
        {
            if (val == lastVal) 
                return;
            lastVal = val;
            action(val);
        }).SetEase(ease).OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener OtimizedTween(Action<int> action, int from, int to, float duration, Ease ease = Ease.InOutQuad)
    {
        return OtimizedTween(action, null, from, to, duration, ease);
    }

    public static Tweener OtimizedTween(Action<Vector3> action, Action onComplete, Vector3 from, Vector3 to, float duration, Ease ease = Ease.InOutQuad)
    {
        Vector3 lastVal = from;
        return DOVirtual.Vector3(from, to, duration, (val) =>
        {
            if (val == lastVal) return;
            lastVal = val;
            action(val);
        }).SetEase(ease).OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener OtimizedTween(Action<Vector3> action, Vector3 from, Vector3 to, float duration, Ease ease = Ease.InOutQuad)
    {
        return OtimizedTween(action, null, from, to, duration, ease);
    }
}