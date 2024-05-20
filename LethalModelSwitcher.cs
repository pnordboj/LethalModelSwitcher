using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalModelSwitcher.Input;
using LethalModelSwitcher.Utils;

namespace LethalModelSwitcher
{
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
            
            if (Instance != null)
            {
                Logger.LogError("LethalModelSwitcher already initialized!");
                return;
            }
            
            Instance = this;
            Logger = base.Logger;
            
            CustomLogging.Log("Lethal Model Switcher is starting initialization...");

            // Step 1: Initialize Keybinds
            CustomLogging.Log("Initializing keybinds...");
            Keybinds.InitializeKeybinds();
            
            // Step 2: Load UI Asset Bundle
            CustomLogging.Log("Loading UI asset bundle...");
            AssetLoader.LoadUIAssetBundle("lethalmodelswitcherbundle");

            // Step 3: Patch all necessary methods
            CustomLogging.Log("Patching methods...");
            harmony.PatchAll();

            CustomLogging.Log("Lethal Model Switcher initialization completed!");
        }
    }
}