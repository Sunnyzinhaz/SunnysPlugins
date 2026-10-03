using System.Collections.Generic;
using Exiled.API.Features.Items;

namespace CustomItems.Core
{
    internal static class CustomItemUseGate
    {
        private static readonly HashSet<ushort> LockedSerials = new();

        internal static bool TryBegin(Item item)
        {
            if (item == null)
                return false;

            return LockedSerials.Add(item.Serial);
        }

        internal static void Clear() => LockedSerials.Clear();
    }
}
