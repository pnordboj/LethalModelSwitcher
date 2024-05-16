using LethalModelSwitcher.Utils;
using UnityEngine;

namespace LethalModelSwitcher.Helper
{
    public static class HelperTools
    {
        public static GameObject LoadUIPrefab(string prefabName)
        {
            var prefab = AssetLoader.LoadUIPrefab(prefabName);
            if (prefab == null)
            {
                LethalModelSwitcher.Logger.LogError($"Failed to load prefab: {prefabName}");
            }
            return prefab;
        }

        public static void ToggleUI(GameObject uiElement, bool state)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(state);
            }
            else
            {
                LethalModelSwitcher.Logger.LogError("UI Element is null. Cannot toggle state.");
            }
        }
    }
}