using System;
using Exiled.API.Features;
using HarmonyLib;
using Hints;
using RueI.API;
using RueI.API.Elements;

namespace CustomItems.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.ShowHint), typeof(string), typeof(HintParameter[]), typeof(HintEffect[]), typeof(float))]
public class ExiledHintPatch
{
    private static readonly Tag HintTag = new("ExiledHintTag");

    public static bool Prefix(Player __instance, string message, HintParameter[] hintParameters, HintEffect[] hintEffects, float duration = 3f)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Suprime hints automáticas do CustomRole/CustomItems do SCP-035.
        string normalized = message.Trim();
        if (normalized.IndexOf("YOU HAVE SPAWNED AS A SCP-035", StringComparison.OrdinalIgnoreCase) >= 0 ||
            normalized.IndexOf("A MÁSCARA POSSESSIVA SCP-035", StringComparison.OrdinalIgnoreCase) >= 0 ||
            normalized.IndexOf("VOCÊ PEGOU O SCP-035", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        RueDisplay display = RueDisplay.Get(__instance);
        BasicElement element = new BasicElement(300, message);
        display.Show(HintTag, element, TimeSpan.FromSeconds(duration));
        return false;
    }
}
