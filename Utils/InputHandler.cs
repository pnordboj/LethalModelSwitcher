using System;
using GameNetcodeStuff;
using UnityEngine;
using HarmonyLib;
using LethalModelSwitcher.Helper;
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
        public static ModelSelectorUI ModelSelectorUI;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void InitializeModelSwitcher(PlayerControllerB __instance)
        {
            __instance.gameObject.AddComponent<InputHandler>();
        }

        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void OnLocalClientReady(PlayerControllerB __instance)
        {
            CustomLogging.Log("Initializing local player.");

            if (ModelSelectorUI == null)
            {
                var modelSelectorPrefab = HelperTools.LoadUIPrefab("LMSCanvas");
                if (modelSelectorPrefab != null)
                {
                    var modelSelectorInstance = Instantiate(modelSelectorPrefab);
                    ModelSelectorUI = modelSelectorInstance.AddComponent<ModelSelectorUI>();
                    ModelSelectorUI.AssignUIElements(modelSelectorInstance);
                    modelSelectorInstance.SetActive(false);
                }
            }
        }

        private static void OnToggleModelKeyPressed(InputAction.CallbackContext context)
        {
            if (context.performed && EnableCycling)
            {
                ToggleModel();
            }
        }

        private static void OnOpenModelSelectorKeyPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OpenModelSelector();
            }
        }

        public static void OpenModelSelector()
        {
            CustomLogging.Log("Opening model selector");

            if (ModelSelectorUI == null)
            {
                CustomLogging.LogError("ModelSelectorUI is not initialized.");
                return;
            }

            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null)
            {
                CustomLogging.LogError("LocalPlayerController is not initialized.");
                return;
            }

            var suitId = localPlayer.currentSuitID;
            string currentModel = ModelManager.GetSuitName(suitId);

            if (!ModelManager.RegisteredModels.ContainsKey(currentModel))
            {
                CustomLogging.LogError($"No registered models found for suit: {currentModel}");
                return;
            }

            if (ModelSelectorUI.lmsCanvasInstance.activeSelf)
            {
                ModelSelectorUI.Close();
                EnableCycling = true;
            }
            else
            {
                ModelSelectorUI.Open(ModelManager.RegisteredModels[currentModel]);
                EnableCycling = false;
            }
        }

        private static PlayerControllerB FindLocalPlayerController()
        {
            foreach (var player in FindObjectsOfType<PlayerControllerB>())
            {
                if (player.IsOwner)
                {
                    return player;
                }
            }
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

            var currentModel = localPlayer.GetComponent<BodyReplacementBase>().suitName;
            if (!ModelManager.RegisteredModels.ContainsKey(currentModel)) return;

            var models = ModelManager.RegisteredModels[currentModel];
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

            var currentModel = localPlayer.GetComponent<BodyReplacementBase>().suitName;
            var models = ModelManager.RegisteredModels[currentModel];

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
