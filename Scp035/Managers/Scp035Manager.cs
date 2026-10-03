using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Lockers;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using Scp035.Extensions;
using Scp035.Models;
using UnityEngine;
using VoiceChat;

namespace Scp035.Managers
{
    internal sealed class Scp035Manager
    {
        private readonly Plugin plugin;
        internal Plugin Plugin => plugin;
        private CoroutineHandle corrosionCoroutine;
        private CoroutineHandle naturalSpawnCoroutine;
        private readonly Dictionary<ushort, string> forbiddenPickupSerials = new();
        private readonly HashSet<string> forbiddenTokens = new(StringComparer.OrdinalIgnoreCase);

        // Estes três itens são bloqueados de forma obrigatória para o SCP-035.
        // Mantemos aqui além da config para configs YAML antigas também receberem o bloqueio.
        private static readonly string[] MandatoryForbiddenPickupNames =
        {
            "SCP-127",
            "Machete",
            "SCP-1509",
        };

        internal Scp035Session Session { get; private set; }
        internal bool IsActive => Session?.Controller != null && !Session.IsEnding;
        internal bool IsBusy { get; private set; }
        internal readonly HashSet<Player> SuppressRagdoll = new();
        internal RoleTypeId HostAppearanceRole => Session?.HostRole ?? RoleTypeId.None;

        public Scp035Manager(Plugin plugin)
        {
            this.plugin = plugin;

            foreach (string token in (plugin.Config.ForbiddenPickupNames ?? Enumerable.Empty<string>())
                         .Concat(MandatoryForbiddenPickupNames))
            {
                string normalized = NormalizeItemName(token);
                if (!string.IsNullOrWhiteSpace(normalized))
                    forbiddenTokens.Add(normalized);
            }
        }

        internal bool Is035(Player player) => IsActive && Session.Controller == player;

        internal bool IsForbiddenVanillaItemType(ItemType itemType)
        {
            if (plugin.Config.ForbiddenVanillaPickupTypes == null)
                return false;

            string current = itemType.ToString();
            return plugin.Config.ForbiddenVanillaPickupTypes.Any(x =>
                !string.IsNullOrWhiteSpace(x) &&
                string.Equals(current, x.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        internal string GetForbiddenVanillaDisplayName(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.MicroHID => "Micro H.I.D.",
                ItemType.Jailbird => "Jailbird",
                ItemType.ParticleDisruptor => "Particle Disruptor",
                _ => itemType.ToString(),
            };
        }

        internal bool IsForbidden035CustomItem(CustomItem customItem)
        {
            if (customItem == null)
                return false;

            return MatchesForbiddenToken(customItem.Name) ||
                   MatchesForbiddenToken(customItem.Description) ||
                   MatchesForbiddenToken(customItem.GetType().Name) ||
                   MatchesForbiddenToken(customItem.GetType().FullName);
        }

        private bool MatchesForbiddenToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string candidate = NormalizeItemName(value);
            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            foreach (string wanted in forbiddenTokens)
            {
                if (candidate.Contains(wanted))
                    return true;
            }

            return false;
        }

        private static string NormalizeItemName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // Sem Regex: esse método pode ser chamado durante interação de pickup.
            // Ignora RichText e mantém apenas letras/números em minúsculo.
            StringBuilder builder = new(value.Length);
            bool insideTag = false;

            foreach (char c in value)
            {
                if (c == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (c == '>')
                {
                    insideTag = false;
                    continue;
                }

                if (insideTag || !char.IsLetterOrDigit(c))
                    continue;

                builder.Append(char.ToLowerInvariant(c));
            }

            return builder.ToString();
        }

        internal bool TryGetForbiddenPickupName(Pickup pickup, out string forbiddenName)
        {
            forbiddenName = null;
            if (pickup == null)
                return false;

            if (IsForbiddenVanillaItemType(pickup.Type))
            {
                forbiddenName = GetForbiddenVanillaDisplayName(pickup.Type);
                return true;
            }

            if (CustomItem.TryGet(pickup, out CustomItem customItem) && IsForbidden035CustomItem(customItem))
            {
                forbiddenName = customItem.Name;
                return true;
            }

            string[] candidates =
            {
                pickup.Type.ToString(),
                pickup.ToString(),
                pickup.Base?.name ?? string.Empty,
                pickup.Base?.GetType().Name ?? string.Empty,
                pickup.Base?.GetType().FullName ?? string.Empty,
            };

            foreach (string candidate in candidates)
            {
                if (!MatchesForbiddenToken(candidate))
                    continue;

                string normalizedCandidate = NormalizeItemName(candidate);
                forbiddenName = (plugin.Config.ForbiddenPickupNames ?? Enumerable.Empty<string>())
                    .Concat(MandatoryForbiddenPickupNames)
                    .FirstOrDefault(x =>
                    {
                        string normalized = NormalizeItemName(x);
                        return !string.IsNullOrWhiteSpace(normalized) && normalizedCandidate.Contains(normalized);
                    })
                    ?? "item bloqueado";
                return true;
            }

            return false;
        }

        internal void RemoveForbiddenItems(Player player)
        {
            if (player == null || !player.IsConnected || !Is035(player))
                return;

            foreach (Item item in player.Items.ToList())
            {
                if (!IsForbidden035Item(item))
                    continue;

                string itemName = IsForbiddenVanillaItemType(item.Type)
                    ? GetForbiddenVanillaDisplayName(item.Type)
                    : "item bloqueado";

                if (CustomItem.TryGet(item, out CustomItem customItem) && !string.IsNullOrWhiteSpace(customItem.Name))
                    itemName = customItem.Name;

                try
                {
                    player.RemoveItem(item);
                    item.Destroy();
                }
                catch (Exception e)
                {
                    Log.Debug($"[SCP-035] Falha removendo item bloqueado: {e.Message}");
                }

                player.ShowHint($"<color=#ff3030>O SCP-035 não pode carregar {itemName}.</color>", 2.5f);
            }
        }

        private bool IsForbidden035Item(Item item)
        {
            if (item == null)
                return false;

            if (IsForbiddenVanillaItemType(item.Type))
                return true;

            if (CustomItem.TryGet(item, out CustomItem customItem) && IsForbidden035CustomItem(customItem))
                return true;

            string[] candidates =
            {
                item.Type.ToString(),
                item.ToString(),
                item.Base?.name ?? string.Empty,
                item.Base?.GetType().Name ?? string.Empty,
                item.Base?.GetType().FullName ?? string.Empty,
            };

            return candidates.Any(MatchesForbiddenToken);
        }

        internal bool IsCachedForbiddenPickup(Pickup pickup, out string forbiddenName)
        {
            forbiddenName = null;
            if (pickup == null)
                return false;

            if (forbiddenPickupSerials.TryGetValue(pickup.Serial, out string cachedName))
            {
                forbiddenName = cachedName;
                return true;
            }

            if (TryGetForbiddenPickupName(pickup, out forbiddenName))
            {
                forbiddenPickupSerials[pickup.Serial] = forbiddenName ?? "item bloqueado";
                return true;
            }

            return false;
        }

        // v4.20 REWORK:
        // Não existe mais scanner global de Pickup.List.
        // O bloqueio de pickups é 100% por evento (SearchingPickup/PickingUpItem),
        // verificando somente o item com o qual o SCP-035 está interagindo.

        private bool IsSpectatorControllerCandidate(Player player, Player host)
        {
            if (player == null || player == host || player.Role.Type != RoleTypeId.Spectator)
                return false;

            if (player.IsConnected)
                return true;

            return plugin.Config.AllowSpectatorDummies && IsDummyLike(player);
        }

        internal static bool IsDummyLike(Player player)
        {
            if (player == null)
                return false;

            // Compatibilidade sem depender de um plugin específico de dummy.
            foreach (string propertyName in new[] { "IsDummy", "IsNPC", "IsNpc", "IsBot" })
            {
                try
                {
                    PropertyInfo property = player.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (property != null &&
                        property.PropertyType == typeof(bool) &&
                        property.GetValue(player) is bool value &&
                        value)
                        return true;
                }
                catch
                {
                    // Ignora APIs que não expõem essa propriedade.
                }
            }

            string userId = player.UserId ?? string.Empty;
            string nickname = player.Nickname ?? string.Empty;

            // Muitos dummies não possuem UserId real.
            if (string.IsNullOrWhiteSpace(userId))
                return true;

            return userId.IndexOf("dummy", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   userId.IndexOf("bot", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   nickname.IndexOf("dummy", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   nickname.IndexOf("bot", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsUsableController(Player player)
        {
            return player != null && (player.IsConnected || IsDummyLike(player));
        }

        internal void BeginPossession(Player host, Item scp035Item)
        {
            if (host == null || scp035Item == null || !host.IsAlive || IsBusy || IsActive)
                return;

            IsBusy = true;
            Timing.RunCoroutine(PossessionRoutine(host, scp035Item));
        }

        private IEnumerator<float> PossessionRoutine(Player host, Item scp035Item)
        {
            yield return Timing.WaitForSeconds(Math.Max(0f, plugin.Config.PossessionDelay));

            if (host == null || !host.IsConnected || !host.IsAlive)
            {
                IsBusy = false;
                yield break;
            }

            if (!CustomItem.TryGet(scp035Item, out var customItem) || customItem.Id != plugin.Config.Scp035ItemId)
            {
                IsBusy = false;
                yield break;
            }

            Vector3 hostPosition = host.Position;
            Vector3 hostEuler = host.Transform.eulerAngles;
            RoleTypeId hostRole = host.Role.Type;

            List<ItemType> inventory = host.Items
                .Where(i => i != scp035Item && !IsForbidden035Item(i))
                .Select(i => i.Type)
                .ToList();
            Dictionary<AmmoType, ushort> ammo = new();
            foreach (AmmoType ammoType in Enum.GetValues(typeof(AmmoType)))
            {
                try { ammo[ammoType] = host.GetAmmo(ammoType); }
                catch { }
            }

            List<Player> spectatorCandidates;

            if (plugin.Config.PullSpectatorOnPossession)
            {
                spectatorCandidates = Player.List
                    .Where(p => IsSpectatorControllerCandidate(p, host))
                    .ToList();

                Log.Info(
                    $"[SCP-035][POSSESSION] Troca com Spectator ATIVADA | " +
                    $"Candidatos={spectatorCandidates.Count} | " +
                    $"Dummies={spectatorCandidates.Count(IsDummyLike)}");
            }
            else
            {
                // Opção desligada: quem usou a máscara permanece como SCP-035.
                // Não consulta/seleciona ninguém do Spectator.
                spectatorCandidates = new List<Player>();

                Log.Info(
                    $"[SCP-035][POSSESSION] Troca com Spectator DESATIVADA | " +
                    $"O hospedeiro {host.Nickname} permanecerá como SCP-035.");
            }

            Player spectator = spectatorCandidates
                .OrderBy(_ => UnityEngine.Random.value)
                .FirstOrDefault();

            if (spectator != null)
            {
                Log.Info(
                    $"[SCP-035][POSSESSION] Controller escolhido: {spectator.Nickname} | " +
                    $"Dummy={IsDummyLike(spectator)} | Connected={spectator.IsConnected} | " +
                    $"Role={spectator.Role.Type}");
            }

            Player controller = spectator ?? host;

            Session = new Scp035Session
            {
                Host = host,
                Controller = controller,
                HostRole = hostRole,
                OldCustomName = controller.CustomName,
                OldDisplayNickname = controller.DisplayNickname,
            };

            try
            {
                if (host.Items.Contains(scp035Item))
                    host.RemoveItem(scp035Item);
                scp035Item.Destroy();
            }
            catch (Exception e)
            {
                Log.Debug($"[SCP-035] Falha não crítica destruindo item: {e.Message}");
            }

            if (spectator != null)
            {
                bool transferred = false;
                bool roleSetOk = true;

                try
                {
                    spectator.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
                }
                catch (Exception e)
                {
                    roleSetOk = false;
                    Log.Error($"[SCP-035] Erro ao colocar spectator como Tutorial: {e}");
                }

                if (roleSetOk)
                {
                    float waited = 0f;
                    while (waited < 1.5f && (!spectator.IsAlive || spectator.Role.Type != RoleTypeId.Tutorial))
                    {
                        yield return Timing.WaitForSeconds(0.1f);
                        waited += 0.1f;
                    }

                    if (IsUsableController(spectator) && spectator.IsAlive && spectator.Role.Type == RoleTypeId.Tutorial)
                    {
                        try
                        {
                            spectator.Position = hostPosition;
                            spectator.Transform.eulerAngles = hostEuler;
                            spectator.ClearInventory();

                            foreach (ItemType type in inventory)
                                spectator.AddItem(type);

                            foreach (var pair in ammo)
                            {
                                try { spectator.SetAmmo(pair.Key, pair.Value); }
                                catch { }
                            }

                            ActivateController(spectator, hostRole);
                            plugin.Hints?.ShowPossessedSpectatorHint(spectator, host);

                            SuppressRagdoll.Add(host);
                            host.Kill(DamageType.Unknown, "Seu corpo foi possuído pelo SCP-035.");
                            Timing.CallDelayed(1f, () => SuppressRagdoll.Remove(host));

                            transferred = true;
                        }
                        catch (Exception e)
                        {
                            Log.Error($"[SCP-035] Erro finalizando transferência para spectator: {e}");
                        }
                    }
                }

                if (!transferred)
                {
                    Log.Warn("[SCP-035] Falha ao transferir controle para um spectator. Recuando para auto-possessão do hospedeiro.");
                    Session.Controller = host;
                    Session.OldCustomName = host.CustomName;
                    Session.OldDisplayNickname = host.DisplayNickname;
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(PrepareHostAsTutorial(host, hostRole, hostPosition, hostEuler, inventory, ammo)));
                }
            }
            else
            {
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(PrepareHostAsTutorial(host, hostRole, hostPosition, hostEuler, inventory, ammo)));
            }

            IsBusy = false;
        }

        private IEnumerator<float> PrepareHostAsTutorial(
            Player host,
            RoleTypeId appearance,
            Vector3 position,
            Vector3 euler,
            List<ItemType> inventory,
            Dictionary<AmmoType, ushort> ammo)
        {
            if (host == null || !host.IsConnected)
                yield break;

            // O 035 usa Tutorial como role REAL. A aparência humana é apenas visual.
            // Isso evita o bloqueio de friendly-fire da equipe humana antes do evento Hurting.
            host.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);

            float waited = 0f;
            while (waited < 1.5f && (!host.IsAlive || host.Role.Type != RoleTypeId.Tutorial))
            {
                yield return Timing.WaitForSeconds(0.1f);
                waited += 0.1f;
            }

            if (!host.IsConnected || !host.IsAlive || host.Role.Type != RoleTypeId.Tutorial)
            {
                Log.Error("[SCP-035] Não foi possível transformar o hospedeiro em Tutorial.");
                yield break;
            }

            host.Position = position;
            host.Transform.eulerAngles = euler;
            host.ClearInventory();

            foreach (ItemType type in inventory)
                host.AddItem(type);

            foreach (var pair in ammo)
            {
                try { host.SetAmmo(pair.Key, pair.Value); }
                catch { }
            }

            ActivateController(host, appearance);
        }

        private void ActivateController(Player player, RoleTypeId appearance)
        {
            if (!IsUsableController(player))
            {
                Log.Error("[SCP-035] Controller inválido durante ActivateController.");
                Reset(false);
                return;
            }

            Timing.CallDelayed(0.15f, () =>
            {
                if (!Is035(player) || !IsUsableController(player))
                    return;

                if (player.Role.Type != RoleTypeId.Tutorial)
                    Log.Warn($"[SCP-035] Controller {player.Nickname} não está como Tutorial (role atual: {player.Role.Type}).");

                player.ChangeAppearance(appearance, true);
                player.MaxHealth = plugin.Config.Scp035RoleConfig.MaxHealth;
                player.Health = plugin.Config.Scp035RoleConfig.MaxHealth;
                player.CustomName = "SCP-035";
                player.DisplayNickname = "SCP-035";
                player.CustomInfo = "SCP-035";
                player.VoiceChannel = VoiceChatChannel.ScpChat;
                player.IsGodModeEnabled = false;
            });

            // A aparência humana é aplicada primeiro. A schematic só nasce depois,
            // para não ser criada durante a pequena janela em que o controller ainda
            // está estabilizando como Tutorial/ChangeAppearance.
            // A source base cria a máscara logo após a posse estabilizar.
            // Mantemos apenas um atraso curto e fixo para o Role/appearance terminar,
            // sem alterar o movimento da máscara.
            Timing.CallDelayed(0.15f, () => SpawnMask(player));
            corrosionCoroutine = Timing.RunCoroutine(Corrosion(player), $"scp035-corrosion-{player.UserId}");

            // Verificação única após a posse. Depois disso o bloqueio é por evento,
            // sem varrer Pickup.List dez vezes por segundo.
            Timing.CallDelayed(0.35f, () =>
            {
                if (Is035(player))
                    RemoveForbiddenItems(player);
            });

            if (!string.IsNullOrWhiteSpace(plugin.Config.BreachCassie))
                Exiled.API.Features.Cassie.Message(plugin.Config.BreachCassie, false, false, true);
        }

        private IEnumerator<float> Corrosion(Player player)
        {
            float interval = Math.Max(0.1f, plugin.Config.Scp035RoleConfig.CorrosionInterval);
            float damage = Math.Max(0f, plugin.Config.Scp035RoleConfig.CorrosionDamage);

            while (Is035(player) && player.IsAlive)
            {
                yield return Timing.WaitForSeconds(interval);
                if (!Is035(player) || !player.IsAlive)
                    yield break;
                player.Hurt(new UniversalDamageHandler(damage, DeathTranslations.Poisoned));
            }
        }

        private void SpawnMask(Player player)
        {
            if (player == null || !Is035(player))
            {
                Log.Warn("[SCP-035][MASK] SpawnMask cancelado: player nulo ou não é 035.");
                return;
            }

            DestroyMask();

            const string schematicName = "035";

            try
            {
                Log.Info(
                    $"[SCP-035][MASK] INICIANDO criação da schematic '{schematicName}' | " +
                    $"Player={player.Nickname} | Role={player.Role.Type} | Pos={player.Position}");

                // Para diagnóstico, nasce na posição do player.
                SchematicObject maskObj = ObjectSpawner.SpawnSchematic(
                    schematicName,
                    player.Position,
                    player.GameObject.transform.localRotation,
                    Vector3.one);

                if (maskObj == null)
                {
                    Log.Error(
                        $"[SCP-035][MASK] FALHA: ObjectSpawner.SpawnSchematic('{schematicName}') retornou NULL. " +
                        "A schematic NÃO foi criada.");
                    return;
                }

                Session.Mask = maskObj;

                int blocks = maskObj.AttachedBlocks?.Count ?? 0;
                int toys = maskObj.AdminToyBases?.Count ?? 0;

                Log.Info(
                    $"[SCP-035][MASK] CRIADA COM SUCESSO | Nome='{schematicName}' | " +
                    $"Blocks={blocks} | AdminToys={toys} | " +
                    $"GameObjectActive={maskObj.gameObject.activeInHierarchy} | " +
                    $"WorldPos={maskObj.transform.position}");

                if (blocks == 0 && toys == 0)
                {
                    Log.Error(
                        $"[SCP-035][MASK] A schematic '{schematicName}' existe, mas veio VAZIA " +
                        "(0 Blocks / 0 AdminToys). Verifique a schematic no ProjectMER.");
                }

                // Por padrão NÃO esconde do próprio 035 durante o teste.
                if (plugin.Config.HideMaskFromController)
                {
                    maskObj.SendFakeScale(player, Vector3.zero);
                    Log.Warn("[SCP-035][MASK] HideMaskFromController=true: máscara escondida do próprio 035.");
                }

                float delay = Mathf.Max(0.10f, plugin.Config.MaskAttachDelay);

                Timing.CallDelayed(delay, () =>
                {
                    if (maskObj == null)
                    {
                        Log.Error("[SCP-035][MASK] ATTACH cancelado: maskObj virou NULL.");
                        return;
                    }

                    if (player == null || !Is035(player))
                    {
                        Log.Warn("[SCP-035][MASK] ATTACH cancelado: jogador não é mais 035.");
                        return;
                    }

                    try
                    {
                        Transform anchor;

                        if (plugin.Config.UseBodyAnchorForMask)
                        {
                            // FALLBACK MAIS SIMPLES/ROBUSTO:
                            // prende no Transform do corpo para garantir que a máscara
                            // pelo menos fique junto do jogador.
                            anchor = player.Transform;

                            maskObj.transform.SetParent(anchor, false);
                            maskObj.transform.localPosition = new Vector3(
                                0f,
                                plugin.Config.MaskBodyY,
                                plugin.Config.MaskBodyZ);
                            maskObj.transform.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            anchor = player.CameraTransform;

                            if (anchor == null)
                            {
                                Log.Error("[SCP-035][MASK] CameraTransform NULL durante attach.");
                                return;
                            }

                            maskObj.transform.SetParent(anchor, false);
                            maskObj.transform.localPosition = new Vector3(
                                plugin.Config.MaskLocalX,
                                plugin.Config.MaskLocalY,
                                plugin.Config.MaskLocalZ);
                            maskObj.transform.localRotation = Quaternion.Euler(
                                plugin.Config.MaskLocalPitch,
                                plugin.Config.MaskLocalYaw,
                                plugin.Config.MaskLocalRoll);
                        }

                        // Publica a pose logo após o parent.
                        maskObj.ForceNetworkPose();

                        MaskFollowerBehaviour sync =
                            maskObj.gameObject.AddComponent<MaskFollowerBehaviour>();
                        sync.Initialize(this, player, maskObj);

                        Log.Info(
                            $"[SCP-035][MASK] ATTACH OK | Anchor={(plugin.Config.UseBodyAnchorForMask ? "BODY" : "CAMERA")} | " +
                            $"Parent={maskObj.transform.parent?.name ?? "NULL"} | " +
                            $"LocalPos={maskObj.transform.localPosition} | " +
                            $"WorldPos={maskObj.transform.position} | " +
                            $"Active={maskObj.gameObject.activeInHierarchy} | Blocks={blocks} | Toys={toys}");

                        float checkDelay = Mathf.Max(0.50f, plugin.Config.MaskDebugCheckDelay);

                        Timing.CallDelayed(checkDelay, () =>
                        {
                            if (maskObj == null)
                            {
                                Log.Error("[SCP-035][MASK-CHECK] maskObj NULL após attach.");
                                return;
                            }

                            int activeBlocks = 0;
                            if (maskObj.AttachedBlocks != null)
                            {
                                foreach (var block in maskObj.AttachedBlocks)
                                {
                                    if (block != null && block.gameObject.activeInHierarchy)
                                        activeBlocks++;
                                }
                            }

                            int activeToys = 0;
                            if (maskObj.AdminToyBases != null)
                            {
                                foreach (var toy in maskObj.AdminToyBases)
                                {
                                    if (toy != null && toy.gameObject.activeInHierarchy)
                                        activeToys++;
                                }
                            }

                            Log.Info(
                                $"[SCP-035][MASK-CHECK] EXISTE={maskObj != null} | " +
                                $"RootActive={maskObj.gameObject.activeInHierarchy} | " +
                                $"Parent={maskObj.transform.parent?.name ?? "NULL"} | " +
                                $"WorldPos={maskObj.transform.position} | " +
                                $"LocalPos={maskObj.transform.localPosition} | " +
                                $"BlocksAtivos={activeBlocks}/{blocks} | ToysAtivos={activeToys}/{toys}");
                        });
                    }
                    catch (Exception e)
                    {
                        Log.Error($"[SCP-035][MASK] ERRO NO ATTACH: {e}");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error($"[SCP-035][MASK] EXCEÇÃO AO CRIAR '035': {e}");
            }
        }

        internal void GetMaskTransform(Player player, Transform body, Transform head, out Vector3 position, out Quaternion rotation)
        {
            float pitch = NormalizeAngle(head.eulerAngles.x);
            float yaw = body.eulerAngles.y;

            Quaternion yawOnly = Quaternion.Euler(0f, yaw, 0f);
            rotation = Quaternion.Euler(
                pitch + plugin.Config.MaskCameraPitch,
                yaw + plugin.Config.MaskCameraYaw,
                plugin.Config.MaskCameraRoll);

            Vector3 baseRoot = player.Position +
                               (body.forward * (plugin.Config.MaskRootForwardOffset + plugin.Config.MaskFaceForwardOffset)) +
                               (body.right * plugin.Config.MaskRootRightOffset) +
                               (Vector3.up * plugin.Config.MaskRootUpOffset);

            Vector3 localPivot = new Vector3(
                plugin.Config.MaskCameraLocalX,
                plugin.Config.MaskPitchPivotHeight + plugin.Config.MaskCameraLocalY,
                plugin.Config.MaskPitchPivotForward + plugin.Config.MaskCameraLocalZ);

            Vector3 pivotWorldAtNeutral = baseRoot + (yawOnly * localPivot);
            position = pivotWorldAtNeutral - (rotation * localPivot);
        }

        private static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }

        internal void On035Died(Player player, Player attacker, DamageType damageType)
        {
            if (!Is035(player) || Session.IsEnding)
                return;

            Session.IsEnding = true;
            string message = BuildContainmentCassie(attacker, damageType);
            Log.Info($"[SCP-035] CASSIE de contenção: {message}");
            Exiled.API.Features.Cassie.Message(message, false, false, true);

            CleanupPlayer(player);
            DestroyMask();
            Timing.CallDelayed(0.1f, () =>
            {
                Session = null;
                IsBusy = false;
            });
        }

        private static string BuildContainmentCassie(Player attacker, DamageType damageType)
        {
            string cause = damageType.ToString().ToLowerInvariant();

            if (cause.Contains("tesla"))
                return "bell_start SCP 0 3 5 contained successfully by Automatic Security System bell_end";

            if (cause.Contains("decontamination") || cause.Contains("decontamin"))
                return "bell_start SCP 0 3 5 lost in Decontamination Sequence bell_end";

            if (cause.Contains("warhead") || cause.Contains("alpha"))
                return "bell_start SCP 0 3 5 terminated by Alpha Warhead bell_end";

            if (attacker == null || !attacker.IsConnected)
                return "bell_start SCP 0 3 5 successfully terminated . termination cause unspecified bell_end";

            switch (attacker.Role.Type)
            {
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.NtfSergeant:
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.NtfSpecialist:
                    // Pedido: não falar Hotel/Foxtrot/Mike-17.
                    // Apenas "Mobile Task Force".
                    return "bell_start SCP 0 3 5 contained successfully by Mobile Task Force bell_end";

                case RoleTypeId.ChaosConscript:
                case RoleTypeId.ChaosMarauder:
                case RoleTypeId.ChaosRepressor:
                case RoleTypeId.ChaosRifleman:
                    return "bell_start SCP 0 3 5 successfully terminated by Chaos Insurgency bell_end";

                case RoleTypeId.Scientist:
                    return "bell_start SCP 0 3 5 successfully terminated by Science Personnel bell_end";

                case RoleTypeId.ClassD:
                    return "bell_start SCP 0 3 5 successfully terminated by Class D personnel bell_end";

                case RoleTypeId.FacilityGuard:
                    return "bell_start SCP 0 3 5 successfully terminated by Security Personnel bell_end";

                default:
                    return "bell_start SCP 0 3 5 successfully terminated . termination cause unspecified bell_end";
            }
        }

        internal void BeginNaturalSpawnDiagnostics()
        {
            Timing.KillCoroutines(naturalSpawnCoroutine);
            naturalSpawnCoroutine = Timing.RunCoroutine(NaturalSpawnDiagnostics(), "scp035-natural-spawn-diagnostics");
        }

        private IEnumerator<float> NaturalSpawnDiagnostics()
        {
            yield return Timing.WaitForSeconds(4f);

            for (int attempt = 0; attempt < 6; attempt++)
            {
                if (TryLogExistingNaturalSpawn())
                    yield break;

                yield return Timing.WaitForSeconds(1f);
            }

            Log.Warn("[SCP-035] Nenhum spawn natural detectado após várias tentativas. Forçando um spawn de segurança.");
            if (TryForceSafetySpawn(out Pickup forcedPickup) && forcedPickup != null)
                LogSpawnLocation(forcedPickup, true);
            else
                Log.Error("[SCP-035] Falha ao forçar spawn de segurança do SCP-035.");
        }

        private bool TryLogExistingNaturalSpawn()
        {
            Pickup pickup = Find035Pickup();
            if (pickup != null)
            {
                LogSpawnLocation(pickup, false);
                return true;
            }

            Player holder = Player.List.FirstOrDefault(pl => pl.Items.Any(i => CustomItem.TryGet(i, out CustomItem ci) && ci.Id == plugin.Config.Scp035ItemId));
            if (holder != null)
            {
                Log.Info($"[SCP-035] O item já havia sido coletado por {holder.Nickname} antes do diagnóstico de spawn.");
                return true;
            }

            return false;
        }

        private Pickup Find035Pickup()
        {
            return Pickup.List.FirstOrDefault(p => CustomItem.TryGet(p, out CustomItem item) && item.Id == plugin.Config.Scp035ItemId);
        }

        private bool TryForceSafetySpawn(out Pickup pickup)
        {
            pickup = null;
            try
            {
                Room room = Room.List
                    .Where(r => r.Zone == ZoneType.LightContainment || r.Zone == ZoneType.HeavyContainment || r.Zone == ZoneType.Entrance)
                    .OrderBy(_ => UnityEngine.Random.value)
                    .FirstOrDefault();

                if (room == null)
                    return false;

                Vector3 spawnPos = room.Position + (Vector3.up * 0.15f);
                return CustomItem.TrySpawn(plugin.Config.Scp035ItemId, spawnPos, out pickup) && pickup != null;
            }
            catch (Exception e)
            {
                Log.Error($"[SCP-035] Erro ao forçar spawn de segurança: {e}");
                return false;
            }
        }

        private void LogSpawnLocation(Pickup pickup, bool forced)
        {
            try
            {
                Locker nearestLocker = Locker.List.OrderBy(l => Vector3.Distance(l.Position, pickup.Position)).FirstOrDefault();
                float lockerDistance = nearestLocker == null ? float.MaxValue : Vector3.Distance(nearestLocker.Position, pickup.Position);
                string roomName = pickup.Room?.Type.ToString() ?? "Sala desconhecida";
                string zoneName = pickup.Room?.Zone.ToString() ?? "Zona desconhecida";
                string prefix = forced ? "[SCP-035] Spawn de segurança" : "[SCP-035] Spawn natural";

                if (nearestLocker != null && lockerDistance <= 3.5f)
                    Log.Info($"{prefix} em ARMÁRIO | Zona: {nearestLocker.Zone} | Tipo: {nearestLocker.Type} | Sala: {roomName} | Posição: {pickup.Position}");
                else
                    Log.Info($"{prefix} no MAPA | Zona: {zoneName} | Sala: {roomName} | Posição: {pickup.Position}");
            }
            catch (Exception e)
            {
                Log.Error($"[SCP-035] Erro ao identificar local de spawn: {e}");
            }
        }

        internal void Reset(bool restoreController)
        {
            if (Session != null)
            {
                if (restoreController && IsUsableController(Session.Controller))
                    CleanupPlayer(Session.Controller);
                DestroyMask();
            }

            Timing.KillCoroutines(corrosionCoroutine);
            Timing.KillCoroutines(naturalSpawnCoroutine);
            forbiddenPickupSerials.Clear();
            Session = null;
            IsBusy = false;
            SuppressRagdoll.Clear();
        }

        private void CleanupPlayer(Player player)
        {
            Timing.KillCoroutines(corrosionCoroutine);
            player.CustomInfo = string.Empty;
            player.DisplayNickname = Session?.OldDisplayNickname;
            player.CustomName = Session?.OldCustomName;
            player.VoiceChannel = VoiceChatChannel.Proximity;
            player.Scale = Vector3.one;
        }

        private void DestroyMask()
        {
            try
            {
                if (Session?.Mask != null)
                    Session.Mask.Destroy();
            }
            catch { }
            if (Session != null)
                Session.Mask = null;
        }
    }
}
