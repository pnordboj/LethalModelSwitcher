using GameNetcodeStuff;
using UnityEngine;
using HarmonyLib;
using LethalModelSwitcher.Managers;
using LethalModelSwitcher.UI;
using LethalNetworkAPI;
using ModelReplacement;
using System.Collections.Generic;

namespace LethalModelSwitcher.Utils
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class InputHandler : MonoBehaviour
    {
        public static bool EnableCycling = true;
        private static Dictionary<string, int> lastSelectedVariants = new Dictionary<string, int>();

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

            if (ModelSelectorUI.Instance == null)
            {
                CustomLogging.LogError("ModelSelectorUI is not initialized, attempting to initialize.");

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

            var modelVariants = ModelManager.GetVariants(suitName);
            if (modelVariants == null || modelVariants.Count == 0)
            {
                CustomLogging.LogError($"No registered models found for suit: {suitName}");
                return;
            }

            if (ModelSelectorUI.lmsCanvasInstance != null)
            {
                if (ModelSelectorUI.lmsCanvasInstance.activeSelf)
                {
                    ModelSelectorUI.Close();
                    EnableCycling = true;
                }
                else
                {
                    ModelSelectorUI.Instance.Open(modelVariants);
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
            if (!ModelManager.BaseModels.ContainsKey(suitName)) return;

            var baseModel = ModelManager.GetBaseModel(suitName);
            var variants = ModelManager.GetVariants(suitName);

            if (variants.Count == 1)
            {
                // Toggle between base model and single variant
                bool isBaseModelActive = baseModel.IsActive;
                int nextIndex = isBaseModelActive ? 1 : 0;

                if (isBaseModelActive)
                {
                    lastSelectedVariants[suitName] = nextIndex;
                }
                else
                {
                    nextIndex = 0; // Switch to base model
                }

                SelectModel(nextIndex, suitName);
            }
            else
            {
                // More than one variant available, open the UI
                OpenModelSelector();
            }
        }

        public static void SelectModel(int index, string suitName)
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

            var baseModel = ModelManager.GetBaseModel(suitName);
            var models = ModelManager.GetVariants(suitName);

            if (index == 0)
            {
                // Activate base model
                ModelReplacementAPI.SetPlayerModelReplacement(localPlayer, baseModel.Type);
                baseModel.SetActive(true);
                models.ForEach(m => m.SetActive(false));
            }
            else
            {
                if (index < 0 || index > models.Count)
                {
                    CustomLogging.LogError($"Model index {index} is out of range for suit: {suitName}");
                    return;
                }

                var nextModel = models[index - 1]; // Adjust index for variants list
                ModelReplacementAPI.SetPlayerModelReplacement(localPlayer, nextModel.Type);
                nextModel.SetActive(true);
                models.ForEach(m => m.SetActive(false));
                baseModel.SetActive(false);

                if (nextModel.Sound != null)
                {
                    Managers.SoundManager.PlaySound(nextModel.Sound, localPlayer.transform.position);
                }

                SyncManager.SendModelChange(localPlayer.GetClientId(), suitName, nextModel.Name);
            }
        }
    }
}
