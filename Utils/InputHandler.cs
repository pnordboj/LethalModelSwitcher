using System;
using GameNetcodeStuff;
using UnityEngine;
using HarmonyLib;
using LethalModelSwitcher.Helper;
using LethalModelSwitcher.Input;
using LethalModelSwitcher.UI;
using LethalModelSwitcher.Utils;
using LethalNetworkAPI;
using ModelReplacement;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Utils
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class InputHandler : MonoBehaviour
    {
        public static bool EnableCycling = true;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void InitializeModelSwitcher(PlayerControllerB __instance)
        {
            CustomLogging.Log("Adding InputHandler component to the player.");
            __instance.gameObject.AddComponent<InputHandler>();
        }

        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void OnLocalClientReady(PlayerControllerB __instance)
        {
            CustomLogging.Log("Initializing local player.");

            // Ensure ModelSelectorUI is initialized
            if (ModelSelectorUI.Instance == null)
            {
                CustomLogging.LogError("ModelSelectorUI is not initialized.");
                
                if (ModelSelectorUI.Instance == null)
                {
                    CustomLogging.LogError("Failed to initialize ModelSelectorUI.");
                }
                else
                {
                    CustomLogging.Log("ModelSelectorUI initialized successfully.");
                }
            }
        }

        public static void OpenModelSelector()
        {
            CustomLogging.Log("Opening model selector");

            if (ModelSelectorUI.Instance == null)
            {
                CustomLogging.LogError("ModelSelectorUI.Instance is not initialized.");
                return;
            }

            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null)
            {
                CustomLogging.LogError("LocalPlayerController is not initialized.");
                return;
            }

            var bodyReplacementBase = localPlayer.GetComponent<BodyReplacementBase>();
            if (bodyReplacementBase == null)
            {
                CustomLogging.LogError("BodyReplacementBase component is not found on the local player.");
                return;
            }

            var suitName = bodyReplacementBase.suitName;
            if (string.IsNullOrEmpty(suitName))
            {
                CustomLogging.LogError("Suit name is null or empty.");
                return;
            }

            if (!ModelManager.RegisteredModels.ContainsKey(suitName))
            {
                CustomLogging.LogError($"No registered models found for suit: {suitName}");
                return;
            }

            // Directly control activation of the lmsCanvasInstance
            if (ModelSelectorUI.lmsCanvasInstance != null)
            {
                if (ModelSelectorUI.lmsCanvasInstance.activeSelf)
                {
                    ModelSelectorUI.Close();
                    EnableCycling = true;
                }
                else
                {
                    ModelSelectorUI.Instance.Open(ModelManager.GetVariants(suitName)); // Get only variants
                    EnableCycling = false;
                }
            }
            else
            {
                CustomLogging.LogError("ModelSelectorUI.lmsCanvasInstance is null.");
            }
        }

        public static PlayerControllerB FindLocalPlayerController()
        {
            foreach (var player in FindObjectsOfType<PlayerControllerB>())
            {
                if (player.IsOwner)
                {
                    return player;
                }
            }
            CustomLogging.LogError("LocalPlayerController not found.");
            return null;
        }

        public static void ToggleModel()
        {
            CustomLogging.Log("Toggling model");

            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null)
            {
                CustomLogging.LogError("LocalPlayerController is not initialized. Cannot toggle model.");
                return;
            }

            var bodyReplacementBase = localPlayer.GetComponent<BodyReplacementBase>();
            if (bodyReplacementBase == null)
            {
                CustomLogging.LogError("BodyReplacementBase component is not found on the local player.");
                return;
            }

            var suitName = bodyReplacementBase.suitName;
            if (!ModelManager.RegisteredModels.ContainsKey(suitName)) return;

            var models = ModelManager.RegisteredModels[suitName];
            var currentIndex = models.FindIndex(m => m.IsActive);
            var nextIndex = (currentIndex + 1) % models.Count;

            SelectModel(nextIndex);
        }

        public static void SelectModel(int index)
        {
            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null)
            {
                CustomLogging.LogError("LocalPlayerController is not initialized. Cannot select model.");
                return;
            }

            var bodyReplacementBase = localPlayer.GetComponent<BodyReplacementBase>();
            if (bodyReplacementBase == null)
            {
                CustomLogging.LogError("BodyReplacementBase component is not found on the local player.");
                return;
            }

            var suitName = bodyReplacementBase.suitName;
            var models = ModelManager.GetVariants(suitName); // Get only variants

            var nextModel = models[index];
            ModelReplacementAPI.SetPlayerModelReplacement(localPlayer, nextModel.Type);
            nextModel.SetActive(true);
            models.ForEach(m => m.SetActive(false));
            nextModel.SetActive(true);

            if (nextModel.Sound != null)
            {
                SoundManager.PlaySound(nextModel.Sound, localPlayer.transform.position);
            }

            SyncManager.SendModelChange(localPlayer.GetClientId(), nextModel.Name);
        }
    }
}
