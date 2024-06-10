using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalModelSwitcher.Utils;

public static class AssetLoader
{
    private static AssetBundle assetBundle;
    private static AssetBundle uiAssetBundle;
    private static AssetBundle networkBundle;
    
    public static void LoadUIAssetBundle(string bundleName)
    {
        string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyLocation, bundleName);

        uiAssetBundle = AssetBundle.LoadFromFile(bundlePath);

        if (uiAssetBundle == null)
        {
            plugin.Logger.LogError("Failed to load UI AssetBundle!");
        }
        else
        {
            plugin.Logger.LogInfo("UI AssetBundle loaded successfully.");
        }
    }

    public static GameObject LoadUIPrefab(string prefabName)
    {
        if (uiAssetBundle == null)
        {
            plugin.Logger.LogError("UI AssetBundle not loaded!");
            return null;
        }

        return uiAssetBundle.LoadAsset<GameObject>(prefabName);
    }

        public static void LoadNetworkBundle(string bundleName)
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string bundlePath = Path.Combine(assemblyLocation, bundleName);

            networkBundle = AssetBundle.LoadFromFile(bundlePath);

            if (networkBundle == null)
            {
                plugin.Logger.LogError("Failed to load Network AssetBundle!");
            }
            else
            {
                plugin.Logger.LogInfo("Network AssetBundle loaded successfully.");
            }
        }

        public static GameObject LoadNetworkPrefab(string prefabName)
        {
            if (networkBundle == null)
            {
                plugin.Logger.LogError("Network AssetBundle not loaded!");
                return null;
            }

            var prefab = networkBundle.LoadAsset<GameObject>(prefabName);
            if (prefab == null)
            {
                plugin.Logger.LogError($"Failed to load Network Prefab: {prefabName} from Network AssetBundle!");
            }
            return prefab;
        }

    public static void LoadAssetBundle(string bundleName)
    {
        string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyLocation, bundleName);

        assetBundle = AssetBundle.LoadFromFile(bundlePath);

        if (assetBundle == null)
        {
            plugin.Logger.LogError("Failed to load AssetBundle!");
        }
        else
        {
            plugin.Logger.LogInfo("AssetBundle loaded successfully.");
        }
    }
    
    public static void LoadLoadedAssetBundle(AssetBundle bundle)
    {
        assetBundle = bundle;

        if (assetBundle == null)
        {
            plugin.Logger.LogError("Failed to load AssetBundle!");
        }
        else
        {
            plugin.Logger.LogInfo("AssetBundle loaded successfully.");
        }
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        if (assetBundle == null)
        {
            plugin.Logger.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<GameObject>(prefabName);
    }

    public static AudioClip LoadAudioClip(string clipName)
    {
        if (assetBundle == null)
        {
            plugin.Logger.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<AudioClip>(clipName);
    }
}