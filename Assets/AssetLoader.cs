using UnityEngine;

namespace LethalModelSwitcher.Assets;

public class AssetLoader
{
    private static AssetBundle assetBundle;

    public static void LoadAssetBundle(string bundleName)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);
        assetBundle = AssetBundle.LoadFromFile(path);

        if (assetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
        }
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        if (assetBundle == null)
        {
            Debug.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<GameObject>(prefabName);
    }

    public static AudioClip LoadAudioClip(string clipName)
    {
        if (assetBundle == null)
        {
            Debug.LogError("AssetBundle not loaded!");
            return null;
        }

        return assetBundle.LoadAsset<AudioClip>(clipName);
    }
}