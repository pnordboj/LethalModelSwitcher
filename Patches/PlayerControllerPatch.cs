using GameNetcodeStuff;
using HarmonyLib;
using LethalModelSwitcher.Managers;
using LethalNetworkAPI;
using ModelReplacement;
using Unity.Netcode;
using UnityEngine;

namespace LethalModelSwitcher.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
    public static class PlayerControllerPatch
    {
        [HarmonyPostfix]
        public static void OnEnablePostfix(PlayerControllerB __instance)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                var networkSync = Object.FindObjectOfType<NetworkSync>();
                if (networkSync == null)
                {
                    CustomLogging.LogError("NetworkSync is null. Cannot sync models.");
                    return;
                }

                foreach (var player in Object.FindObjectsOfType<PlayerControllerB>())
                {
                    if (ModelReplacementAPI.GetPlayerModelReplacement(player, out var modelReplacement))
                    {
                        var suitName = player.GetComponent<BodyReplacementBase>()?.suitName;
                        if (!string.IsNullOrEmpty(suitName) && modelReplacement != null)
                        {
                            networkSync.ChangeModel(player.GetClientId(), suitName, modelReplacement.GetType().Name);
                        }
                    }
                }
            }
        }
    }
}