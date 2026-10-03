using System;
using System.Reflection;
using AdminToys;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Mirror;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace Scp035.Extensions;

public static class SchematicExtensions
{
    private static readonly PropertyInfo NetworkPositionProperty =
        typeof(AdminToyBase).GetProperty("NetworkPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static readonly PropertyInfo NetworkRotationProperty =
        typeof(AdminToyBase).GetProperty("NetworkRotation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


    public static void SendFakeScale(this SchematicObject schematicObj, Player player, Vector3 scale)
    {
        foreach (var block in schematicObj.AttachedBlocks)
        {
            if (block.TryGetComponent<PrimitiveObjectToy>(out var _))
            {
                MirrorExtensions.SendFakeSyncVar(
                    player,
                    block.GetComponent<NetworkIdentity>(),
                    block.GetComponent<PrimitiveObjectToy>().GetType(),
                    "NetworkScale",
                    scale);
            }
        }
    }

    /// <summary>
    /// Remove o intervalo extra de sincronização dos AdminToys da schematic.
    /// O servidor ainda fica limitado pelo tick/rede do jogo, mas evita acumular
    /// atraso adicional no próprio objeto.
    /// </summary>
    /// <summary>
    /// O ProjectMER movimenta o root Unity da schematic, mas os clientes enxergam
    /// os blocos através dos SyncVars de AdminToyBase. Atualizar esses SyncVars junto
    /// com o root reduz drasticamente o atraso visual ao seguir um jogador.
    /// </summary>
    public static void ForceNetworkPose(this SchematicObject schematicObj)
    {
        if (schematicObj == null)
            return;

        foreach (AdminToyBase toy in schematicObj.AdminToyBases)
        {
            if (toy == null)
                continue;

            try
            {
                Transform transform = toy.transform;

                if (NetworkPositionProperty != null && NetworkPositionProperty.CanWrite)
                    NetworkPositionProperty.SetValue(toy, transform.position);

                if (NetworkRotationProperty != null && NetworkRotationProperty.CanWrite)
                {
                    Type rotationType = NetworkRotationProperty.PropertyType;

                    if (rotationType == typeof(Quaternion))
                        NetworkRotationProperty.SetValue(toy, transform.rotation);
                    else if (rotationType == typeof(Vector3))
                        NetworkRotationProperty.SetValue(toy, transform.eulerAngles);
                }
            }
            catch
            {
                // Se um tipo específico de toy não expuser esses SyncVars, ele apenas
                // continua usando a sincronização padrão do ProjectMER.
            }
        }
    }
}
