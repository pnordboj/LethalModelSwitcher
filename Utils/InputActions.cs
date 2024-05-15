using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LethalModelSwitcher.Utils;

public class InputActions : LcInputActions
{
    public static InputActions Instance = new InputActions();

    [InputAction("<Keyboard>/u", Name = "Toggle Model")]
    public InputAction ToggleModelKey { get; set; }

    [InputAction("<Keyboard>/-", Name = "Open Model Selector")]
    public InputAction OpenModelSelectorKey { get; set; }
}