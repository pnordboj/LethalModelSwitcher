using LethalCompanyInputUtils.Api;
using LethalModelSwitcher.Utils;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Input
{
    public class ModelSwitcherInputActions : LcInputActions
    {
        [InputAction("<Keyboard>/u", Name = "Toggle Model")]
        public InputAction ToggleModelAction { get; private set; }

        [InputAction("<Keyboard>/k", Name = "Open Model Selector")]
        public InputAction OpenModelSelectorAction { get; private set; }
    }

    internal static class Keybinds
    {
        public static ModelSwitcherInputActions InputActionsInstance { get; private set; }

        static Keybinds()
        {
            InitializeKeybinds();
        }

        public static void InitializeKeybinds()
        {
            if (InputActionsInstance != null) return;

            CustomLogging.Log("Initializing keybinds...");
            InputActionsInstance = new ModelSwitcherInputActions();

            if (InputActionsInstance.ToggleModelAction == null)
            {
                CustomLogging.LogError("ToggleModelAction is null.");
            }
            else
            {
                CustomLogging.Log("ToggleModelAction initialized.");
            }

            if (InputActionsInstance.OpenModelSelectorAction == null)
            {
                CustomLogging.LogError("OpenModelSelectorAction is null.");
            }
            else
            {
                CustomLogging.Log("OpenModelSelectorAction initialized.");
            }

            InputActionsInstance.ToggleModelAction.performed += context => InputHandler.ToggleModel();
            InputActionsInstance.OpenModelSelectorAction.performed += context => InputHandler.OpenModelSelector();

            InputActionsInstance.Asset.Enable();
            CustomLogging.Log("Keybinds enabled.");
        }
    }
}