using Exiled.API.Features;
using ProjectMER.Features.Objects;
using Scp035.Extensions;
using UnityEngine;

namespace Scp035.Managers
{
    internal sealed class MaskFollowerBehaviour : MonoBehaviour
    {
        private Scp035Manager manager;
        private Player player;
        private SchematicObject mask;
        private float nextSync;
        private float interval;

        internal void Initialize(Scp035Manager manager, Player player, SchematicObject mask)
        {
            this.manager = manager;
            this.player = player;
            this.mask = mask;

            interval = Mathf.Clamp(manager.Plugin.Config.MaskSyncInterval, 0.10f, 0.50f);
            nextSync = 0f;

            ForceSync();
        }

        private void Update()
        {
            if (manager == null || player == null || mask == null || !manager.Is035(player))
            {
                Destroy(this);
                return;
            }

            if (Time.unscaledTime < nextSync)
                return;

            nextSync = Time.unscaledTime + interval;
            ForceSync();
        }

        private void ForceSync()
        {
            try
            {
                mask?.ForceNetworkPose();
            }
            catch
            {
            }
        }
    }
}
