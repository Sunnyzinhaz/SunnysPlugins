namespace CustomItems;

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using YamlDotNet.Serialization;

/// <summary>
/// The plugin's config class.
/// </summary>
public class Config : IConfig
{
    /// <summary>
    /// Gets item Config settings.
    /// </summary>
    [YamlIgnore]
    public Configs.Items ItemConfigs { get; private set; } = null!;

    /// <inheritdoc/>
    [Description("Whether or not this plugin is enabled.")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether if debug mode is enabled.
    /// </summary>
    [Description("Whether or not debug messages should be displayed in the server console.")]
    public bool Debug { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating what subclasses should get what items, and their spawn chances.
    /// </summary>
    [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
    public Dictionary<string, Dictionary<string, float>> SubclassList { get; set; } = new()
    {
        {
            "ExampleSubclass", new Dictionary<string, float> { { "SR-119", 100 }, { "SG-119", 50 } }
        },
    };

    /// <summary>
    /// Gets or sets a value indicating what folder item configs will be stored in.
    /// </summary>
    public string ItemConfigFolder { get; set; } = Path.Combine(Paths.Configs, "CustomItems");

    /// <summary>
    /// Gets or sets a value indicating what file will be used for item configs.
    /// </summary>
    public string ItemConfigFile { get; set; } = "global.yml";

    /// <summary>
    /// Gets or sets a value indicating what role id the Scp035 role has.
    /// This is used to identify the Scp035 role in the CustomRole API.
    /// </summary>
    public uint Scp035RoleId { get; set; } = 51;

    [Description("Quantidade de cada SCP-500 custom que o spawner tentará criar por rodada.")]
    public int Scp500CopiesPerType { get; set; } = 1;

    [Description("Chance, em porcentagem, de cada tipo de SCP-500 custom aparecer na rodada. Valor baixo por padrão porque elas agora só aparecem em armários.")]
    public float Scp500SpawnChancePerType { get; set; } = 10f;

    [Description("LEGADO: SCP-500 agora spawna somente em armários. Mantido para não quebrar configs antigas.")]
    public float Scp500LockerSpawnChance { get; set; } = 100f;

    [Description("Chance, em porcentagem, do SCP-035 ir para um armário quando ele for spawnar naturalmente.")]
    public float Scp035LockerSpawnChance { get; set; } = 75f;

    [Description("Raio máximo de variação do spawn no chão em relação ao centro de uma sala aleatória.")]
    public float RandomGroundSpawnRadius { get; set; } = 0.40f;

    /// <summary>
    /// Loads the item configs.
    /// </summary>
    public void LoadItems()
    {
        if (!Directory.Exists(ItemConfigFolder))
            Directory.CreateDirectory(ItemConfigFolder);

        string filePath = Path.Combine(ItemConfigFolder, ItemConfigFile);
        Log.Debug($"Config file path: {filePath}");

        if (!File.Exists(filePath))
        {
            ItemConfigs = new Configs.Items();
            File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            return;
        }
        
        ItemConfigs = Loader.Deserializer.Deserialize<Configs.Items>(File.ReadAllText(filePath));
        File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
    }

    [Description("Ativa o spawn natural automático das SCP-500 no início da rodada. DESLIGADO por padrão para teste de TPS.")]
    public bool EnableNaturalScp500Spawn { get; set; } = false;

}