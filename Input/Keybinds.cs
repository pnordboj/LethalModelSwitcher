using LethalModelSwitcher.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Input
{
    internal static class Keybinds
    {
        public static InputAction ToggleModelAction { get; private set; }
        public static InputAction OpenModelSelectorAction { get; private set; }

        private static InputActionMap _keybinds;
        private static InputActionAsset _inputActionAsset;

        static Keybinds()
        {
            InitializeKeybinds();
        }

        public static void InitializeKeybinds()
        {
            if (_inputActionAsset != null) return;

            CustomLogging.Log("Initializing keybinds...");

            _inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            _keybinds = _inputActionAsset.AddActionMap("Keybinds");

            ToggleModelAction = _keybinds.AddAction("ToggleModel", binding: "<Keyboard>/u");
            OpenModelSelectorAction = _keybinds.AddAction("OpenModelSelector", binding: "<Keyboard>/k");

            ToggleModelAction.performed += context => InputHandler.ToggleModel();
            OpenModelSelectorAction.performed += context => InputHandler.OpenModelSelector();

            _inputActionAsset.Enable();
        }
    }
}