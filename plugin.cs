using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalModelSwitcher.Input;
using LethalModelSwitcher.Patches;
using LethalModelSwitcher.Utils;
using UnityEngine;

namespace LethalModelSwitcher
{
    [BepInPlugin("nordbo.LethalModelSwitcher", "Lethal Model Switcher", "0.1.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)] // Handling Model Replacements for Suits
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)] // Handling Key Bindings
    [BepInDependency("LethalNetworkAPI", BepInDependency.DependencyFlags.HardDependency)] // Handling Networking
    public class plugin : BaseUnityPlugin
    {
        public static plugin Instance { get; private set; }
        public new static ManualLogSource Logger { get; private set; }
        private readonly Harmony harmony = new Harmony("nordbo.LethalModelSwitcher");

        // Boolean to control the UI
        public bool loadUI = false; // Boolean to control the UI

        private void Awake()
        {
            // Step 1: Initialize Keybinds
            CustomLogging.Log("Initializing keybinds...");
            Keybinds.InitializeKeybinds();

            // Step 2: Load UI Asset Bundle if loadUI is true
            if (loadUI)
            {
                CustomLogging.Log("Loading UI asset bundle...");
                AssetLoader.LoadUIAssetBundle("lmsbundle");
            }

            // Step 3: Initialize the Network
            NetcodePatcher();

            // Step 4: Initialize the Lethal Model Switcher
            if (Instance != null)
            {
                Logger.LogError("LethalModelSwitcher already initialized!");
                return;
            }

            Instance = this;
            Logger = base.Logger;

            CustomLogging.Log("Lethal Model Switcher is starting initialization...");

            // Step 5: Patch all necessary methods
            CustomLogging.Log("Patching methods...");
            harmony.PatchAll();

            CustomLogging.Log("Lethal Model Switcher initialization completed!");
        }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
