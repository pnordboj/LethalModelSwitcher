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
            _inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            _keybinds = _inputActionAsset.AddActionMap("Keybinds");

            ToggleModelAction = _keybinds.AddAction("ToggleModel", binding: "<Keyboard>/u");
            OpenModelSelectorAction = _keybinds.AddAction("OpenModelSelector", binding: "<Keyboard>/k");

            ToggleModelAction.performed += context => InputHandler.ToggleModel();
            OpenModelSelectorAction.performed += context => InputHandler.OpenModelSelector();

            _inputActionAsset.Enable();
        }

        public static void Enable()
        {
            _keybinds.Enable();
        }

        public static void Disable()
        {
            _keybinds.Disable();
        }
    }
}