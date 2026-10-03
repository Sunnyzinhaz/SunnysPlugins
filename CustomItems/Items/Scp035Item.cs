using System;
using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.Core.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace CustomItems.Items
{
    [CustomItem(ItemType.SCP1344)]
    public sealed class Scp035Item : CustomItem, ICustomItemGlow
    {
        private float roundSpawnChance = 100f;

        private SpawnProperties spawnProperties = new()
        {
            // O ServerHandler escolhe aleatoriamente entre chão e armário.
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(),
            LockerSpawnPoints = new List<LockerSpawnPoint>(),
        };

        public override uint Id { get; set; } = 99;
        public override string Name { get; set; } = "SCP-035";

        public override string Description { get; set; } =
            "<color=#ff3030>SCP-035</color>\n" +
            "<color=#ffd84a>Ao utilizar este objeto, ele possuirá seu corpo.</color>";

        public override float Weight { get; set; } = 0.75f;

        [Description("Chance TOTAL, em porcentagem, do SCP-035 aparecer naturalmente em uma rodada. 0 = nunca; 100 = sempre. O comando 035spawn ignora esta chance.")]
        public float RoundSpawnChance
        {
            get => roundSpawnChance;
            set => roundSpawnChance = Math.Max(0f, Math.Min(100f, value));
        }

        // O EXILED avalia a chance de cada DynamicSpawnPoint separadamente.
        // Para que RoundSpawnChance represente a chance TOTAL da rodada, convertemos
        // a probabilidade total para uma chance equivalente por ponto:
        // P(total) = 1 - (1 - P(ponto))^N
        // Limit = 1 garante que no máximo uma máscara seja criada.
        public override SpawnProperties SpawnProperties
        {
            get
            {
                ApplyRoundSpawnChance();
                return spawnProperties;
            }
            set
            {
                spawnProperties = value ?? new SpawnProperties();
                ApplyRoundSpawnChance();
            }
        }

        private void ApplyRoundSpawnChance()
        {
            if (spawnProperties == null)
                spawnProperties = new SpawnProperties();

            spawnProperties.Limit = 1;

            int dynamicCount = spawnProperties.DynamicSpawnPoints?.Count ?? 0;
            int lockerCount = spawnProperties.LockerSpawnPoints?.Count ?? 0;
            int count = dynamicCount + lockerCount;

            if (count == 0)
                return;

            double totalProbability = roundSpawnChance / 100d;

            float pointChance;
            if (roundSpawnChance <= 0f)
            {
                pointChance = 0f;
            }
            else if (roundSpawnChance >= 100f)
            {
                pointChance = 100f;
            }
            else
            {
                pointChance = (float)((1d - Math.Pow(1d - totalProbability, 1d / count)) * 100d);
            }

            if (spawnProperties.DynamicSpawnPoints != null)
            {
                foreach (DynamicSpawnPoint spawnPoint in spawnProperties.DynamicSpawnPoints)
                    spawnPoint.Chance = pointChance;
            }

            if (spawnProperties.LockerSpawnPoints != null)
            {
                foreach (LockerSpawnPoint spawnPoint in spawnProperties.LockerSpawnPoints)
                    spawnPoint.Chance = pointChance;
            }
        }

        // O EXILED CustomItems mostra automaticamente nome/descrição ao pegar e selecionar.
        // O SCP-035 já tem suas próprias hints via Scp035HintManager, então suprimimos
        // as mensagens automáticas para não sobrepor a interface bonita.
        protected override void ShowPickedUpMessage(Exiled.API.Features.Player player) { }
        protected override void ShowSelectedMessage(Exiled.API.Features.Player player) { }

        // Glow: usa a infraestrutura ICustomItemGlow/GlowEventsHandler existente.
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new(1f, 0.05f, 0.05f);
        public float GlowRange { get; set; } = 0.55f;
        public float GlowIntensity { get; set; } = 0.45f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.Soft;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
    }
}
