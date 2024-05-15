using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalModelSwitcher.Utils;

namespace LethalModelSwitcher;

[BepInPlugin("nordbo.LethalModelSwitcher", "Lethal Model Switcher", "0.1.0")]
[BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)] // Handling Model Replacements for Suits
[BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)] // Handling Key Bindings
[BepInDependency("LethalNetworkAPI")] // Handling Networking
public class LethalModelSwitcher : BaseUnityPlugin
{
    public static LethalModelSwitcher Instance { get; private set; }
    public new static ManualLogSource Logger { get; private set; }
    private readonly Harmony harmony = new Harmony("nordbo.LethalModelSwitcher");

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        
        AssetLoader.LoadUIAssetBundle("lethalmodelswitchingbundle");

        harmony.PatchAll();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}
