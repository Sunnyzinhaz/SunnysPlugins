using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace CustomItems.Core.Extensions;

public static class RememberExtensions
{
    /// <summary>
    /// Salva um efeito para ser restaurado posteriormente.
    /// </summary>
    /// <param name="owner">O jogador que terá o efeito salvo.</param>
    /// <param name="effect">O efeito a ser salvo.</param>
    /// <returns>O efeito salvo.</returns>
    public static RememberEffect RememberEffect(this Player owner, EffectType type)
    {
        StatusEffectBase effect = owner.ReferenceHub.playerEffectsController.AllEffects.FirstOrDefault(e => e.GetEffectType() == type);
        return new RememberEffect(owner, effect);
    }
}