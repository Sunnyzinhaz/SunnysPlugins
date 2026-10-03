using System;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace CustomItems.Core;


public class RememberEffect : Effect
{
    public Player Owner { get; set; }

    public RememberEffect(Player owner, EffectType type, byte intensity, float duration) : base(type, duration, intensity)
    {
        Owner = owner;
    }

    public RememberEffect(Player owner, Effect effect) : base(effect.Type, effect.Duration, effect.Intensity)
    {
        Owner = owner;
    }

    public RememberEffect(Player owner, StatusEffectBase effect) : base(effect.GetEffectType(), effect.Duration, effect.Intensity)
    {
        Owner = owner;
    }
    
    public RememberEffect(Player owner, EffectType type)
    {
        StatusEffectBase effect = owner.ReferenceHub.playerEffectsController.AllEffects.FirstOrDefault(e => e.GetEffectType() == type);
        
        Owner = owner;
        Type = type;
        Intensity = effect.Intensity;
        Duration = effect.Duration;
    }

    public void Restore(Player player)
    {
        player.ChangeEffectIntensity(Type, Intensity, Duration);
    }

    public void Restore()
    {
        Restore(Owner);
    }
}