using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Input
{
    internal class IngameKeybinds : LcInputActions
    {
        internal static IngameKeybinds Instance = new IngameKeybinds();

        [InputAction("<Keyboard>/u", Name = "Toggle Model")]
        public InputAction ToggleModelKey { get; set; }

        [InputAction("<Keyboard>/k", Name = "Open Model Selector")]
        public InputAction OpenModelSelectorKey { get; set; }
    }
}