using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Input
{
    internal class IngameKeybinds : LcInputActions
    {
        private static IngameKeybinds InputActions;
        
        public static IngameKeybinds Instance => InputActions ?? (InputActions = new IngameKeybinds());

        [InputAction("<Keyboard>/u", Name = "Toggle Model")]
        public InputAction ToggleModelKey { get; set; }

        [InputAction("<Keyboard>/k", Name = "Open Model Selector")]
        public InputAction OpenModelSelectorKey { get; set; }
    }
}