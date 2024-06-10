using GameNetcodeStuff;
using UnityEngine;
using HarmonyLib;
using LethalModelSwitcher.Managers;
using LethalModelSwitcher.UI;
using ModelReplacement;
using System.Collections.Generic;
using LethalNetworkAPI;

namespace LethalModelSwitcher.Utils
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class InputHandler : MonoBehaviour
    {
        public static bool EnableCycling = true;
        private static Dictionary<string, int> lastSelectedVariants = new Dictionary<string, int>();
        private static bool isToggling = false;
        private static float lastToggleTime = 0f;
        private const float toggleCooldown = 0.5f;

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
                CustomLogging.LogError("ModelSelectorUI is not initialized.");
            }
        }

        public static void OpenModelSelector()
        {
            CustomLogging.Log("Opening model selector");
            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null) return;

            var suitName = localPlayer.GetComponent<BodyReplacementBase>()?.suitName;
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

        public static void ToggleModel()
        {
            if (isToggling || Time.time - lastToggleTime < toggleCooldown)
            {
                CustomLogging.Log("ToggleModel: Attempt to toggle too soon or toggle already in progress.");
                return;
            }

            CustomLogging.Log("Toggling model");
            isToggling = true;
            lastToggleTime = Time.time;

            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null)
            {
                isToggling = false;
                return;
            }

            var bodyReplacementBase = localPlayer.GetComponent<BodyReplacementBase>();
            if (bodyReplacementBase == null)
            {
                CustomLogging.LogError("BodyReplacementBase component is not found on the local player.");
                isToggling = false;
                return;
            }

            var suitName = bodyReplacementBase.suitName;
            if (string.IsNullOrEmpty(suitName))
            {
                CustomLogging.LogError("Suit name is null or empty in ToggleModel.");
                isToggling = false;
                return;
            }

            var baseModel = ModelManager.GetBaseModel(suitName);
            var variants = ModelManager.GetVariants(suitName);

            if (variants != null && variants.Count > 0)
            {
                bool isBaseModelActive = baseModel.IsActive;
                int nextIndex = isBaseModelActive ? 1 : 0;
                SelectModel(nextIndex, suitName);
                bodyReplacementBase.name = nextIndex == 0 ? baseModel.Name : variants[nextIndex - 1].Name;
            }
            else
            {
                OpenModelSelector();
            }

            CustomLogging.Log($"ToggleModel: suitName={suitName}, currentModelName={bodyReplacementBase.name}");
            NetworkSync.Instance.ChangeModel(localPlayer.GetClientId(), suitName, bodyReplacementBase.name);

            isToggling = false;
        }
        
        public static PlayerControllerB FindLocalPlayerController()
        {
            foreach (var player in Object.FindObjectsOfType<PlayerControllerB>())
            {
                if (player.IsOwner)
                {
                    return player;
                }
            }
            CustomLogging.LogError("LocalPlayerController not found.");
            return null;
        }

        public static void SelectModel(int index, string suitName)
        {
            var localPlayer = FindLocalPlayerController();
            if (localPlayer == null) return;

            var bodyReplacementBase = localPlayer.GetComponent<BodyReplacementBase>();
            if (bodyReplacementBase == null) return;

            var baseModel = ModelManager.GetBaseModel(suitName);
            var models = ModelManager.GetVariants(suitName);

            if (index == 0)
            {
                ModelReplacementAPI.SetPlayerModelReplacement(localPlayer, baseModel.Type);
                baseModel.SetActive(true);
                models?.ForEach(m => m.SetActive(false));
            }
            else
            {
                if (index < 0 || index > (models?.Count ?? 0)) return;

                var nextModel = models[index - 1];
                ModelReplacementAPI.SetPlayerModelReplacement(localPlayer, nextModel.Type);
                nextModel.SetActive(true);
                models?.ForEach(m => m.SetActive(false));
                baseModel.SetActive(false);

                if (nextModel.Sound != null)
                {
                    Managers.SoundManager.PlaySound(nextModel.Sound, localPlayer.transform.position);
                }

                NetworkSync.Instance.ChangeModel(localPlayer.GetClientId(), suitName, nextModel.Name);
            }
        }
    }
}
