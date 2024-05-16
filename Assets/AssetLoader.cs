using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalModelSwitcher.Utils;

public static class AssetLoader
{
    private static AssetBundle assetBundle;
    private static AssetBundle uiAssetBundle;
    
    public static void LoadUIAssetBundle(string bundleName)
    {
        string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyLocation, bundleName);

        uiAssetBundle = AssetBundle.LoadFromFile(bundlePath);

        if (uiAssetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("Failed to load UI AssetBundle!");
        }
        else
        {
            LethalModelSwitcher.Logger.LogInfo("UI AssetBundle loaded successfully.");
        }
    }

    public static GameObject LoadUIPrefab(string prefabName)
    {
        if (uiAssetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("UI AssetBundle not loaded!");
            return null;
        }

        return uiAssetBundle.LoadAsset<GameObject>(prefabName);
    }

    public static void LoadAssetBundle(string bundleName)
    {
        string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyLocation, bundleName);

        assetBundle = AssetBundle.LoadFromFile(bundlePath);

        if (assetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("Failed to load AssetBundle!");
        }
        else
        {
            LethalModelSwitcher.Logger.LogInfo("AssetBundle loaded successfully.");
        }
    }
    
    public static void LoadLoadedAssetBundle(AssetBundle bundle)
    {
        assetBundle = bundle;

        if (assetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("Failed to load AssetBundle!");
        }
        else
        {
            LethalModelSwitcher.Logger.LogInfo("AssetBundle loaded successfully.");
        }
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        if (assetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<GameObject>(prefabName);
    }

    public static AudioClip LoadAudioClip(string clipName)
    {
        if (assetBundle == null)
        {
            LethalModelSwitcher.Logger.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<AudioClip>(clipName);
    }
}