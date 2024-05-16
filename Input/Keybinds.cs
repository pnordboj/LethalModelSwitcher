using BepInEx;
using HarmonyLib;
using LethalModelSwitcher.Utils;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Input
{
    [HarmonyPatch]
    internal class Keybinds
    {
        internal static Keybinds Instance;
        
        public InputAction ToggleModelAction { get; private set; }
        public InputAction OpenModelSelectorAction { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeKeybinds();
        }

        private void InitializeKeybinds()
        {
            var keybinds = new InputActionMap("LethalModelSwitcher");
            
            

            ToggleModelAction = keybinds.AddAction("ToggleModel", binding: "<Keyboard>/u");
            OpenModelSelectorAction = keybinds.AddAction("OpenModelSelector", binding: "<Keyboard>/k");

            ToggleModelAction.performed += OnToggleModelPerformed;
            OpenModelSelectorAction.performed += OnOpenModelSelectorPerformed;

            keybinds.Enable();
        }

        private void OnToggleModelPerformed(InputAction.CallbackContext context)
        {
            InputHandler.ToggleModel();
        }

        private void OnOpenModelSelectorPerformed(InputAction.CallbackContext context)
        {
            InputHandler.OpenModelSelector();
        }
    }
}