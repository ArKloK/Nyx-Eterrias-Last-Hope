using UnityEngine;

public static class InputManager
{
    public static PlayerInputActions PlayerInputActions { get; private set; }

    // Inicializador estático
    static InputManager()
    {
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Player.Enable();
        PlayerInputActions.UI.Disable();
    }

    // Métodos para habilitar/deshabilitar esquemas de entrada
    public static void EnablePlayerInput()
    {
        PlayerInputActions.Player.Enable();
        PlayerInputActions.UI.Disable();
    }

    public static void EnableUIInput()
    {
        PlayerInputActions.UI.Enable();
        PlayerInputActions.Player.Disable();
    }
    public static void EnableAllInput()
    {
        PlayerInputActions.Enable();
    }
    public static void DisableAllInput()
    {
        PlayerInputActions.Disable();
    }
}
