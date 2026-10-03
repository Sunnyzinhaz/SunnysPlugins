using System;
using System.Reflection;
using Exiled.API.Features;

namespace CustomItems.Utils;

public static class Scp035Check
{
    private static Type pluginType;
    private static MethodInfo is035Method;

    public static bool IsScp035(this Player player)
    {
        if (player == null)
            return false;

        try
        {
            pluginType ??= Type.GetType("Scp035.Plugin, Scp035");
            is035Method ??= pluginType?.GetMethod("IsScp035Player", BindingFlags.Public | BindingFlags.Static);
            if (is035Method != null)
                return is035Method.Invoke(null, new object[] { player }) is bool result && result;
        }
        catch { }

        // Fallback para não quebrar as SCP-500 caso o plugin ainda esteja carregando.
        return string.Equals(player.CustomName, "SCP-035", StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryGetScp035Role(out Type scp035Role)
    {
        scp035Role = Type.GetType("Scp035.Scp035Role, Scp035");
        return scp035Role != null;
    }

    public static bool TryGetScp035Role() => TryGetScp035Role(out _);

    public static bool IsScp035Role(object customRole) => false;
}
