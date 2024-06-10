using HarmonyLib;
using LethalModelSwitcher.Managers;
using UnityEngine;
using Unity.Netcode;

namespace LethalModelSwitcher.Patches
{
    [HarmonyPatch]
    public static class NetworkObjectManager
    {
        private static GameObject networkPrefab;

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        public static void Init()
        {
            if (networkPrefab != null)
                return;

            var dllFolderPath = System.IO.Path.GetDirectoryName(typeof(NetworkObjectManager).Assembly.Location);
            var assetBundleFilePath = System.IO.Path.Combine(dllFolderPath, "lmsnetworkbundle");
            CustomLogging.Log($"Loading AssetBundle from path: {assetBundleFilePath}");

            var mainAssetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
            if (mainAssetBundle == null)
            {
                CustomLogging.LogError("Failed to load main AssetBundle!");
                return;
            }

            CustomLogging.Log("Main AssetBundle loaded successfully.");

            networkPrefab = mainAssetBundle.LoadAsset<GameObject>("LMSNetworkSyncPrefab");
            if (networkPrefab == null)
            {
                CustomLogging.LogError("Failed to load NetworkSyncPrefab from AssetBundle!");
                return;
            }

            networkPrefab.AddComponent<NetworkSync>();
            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
            CustomLogging.Log("NetworkSyncPrefab added to NetworkManager.");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        public static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
