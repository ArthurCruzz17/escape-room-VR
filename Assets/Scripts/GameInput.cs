using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class GameInput
{
    private const float GamepadLookSensitivity = 70f;

    public static bool GetStartGameDown(KeyCode fallbackKey = KeyCode.E)
    {
        if (GetGamepadStartDown()) return true;
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame) return true;
#endif
        return Input.GetKeyDown(fallbackKey);
    }

    public static bool GetGamepadStartDown()
    {
#if ENABLE_INPUT_SYSTEM
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            return true;
        }

        var gamepads = Gamepad.all;
        for (int i = 0; i < gamepads.Count; i++)
        {
            Gamepad pad = gamepads[i];
            if (pad != null && pad.startButton.wasPressedThisFrame)
            {
                return true;
            }
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            return true;
        }

        for (KeyCode key = KeyCode.Joystick1Button7; key <= KeyCode.Joystick8Button7; key++)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
#endif

        return false;
    }

    public static Vector2 GetMoveVector()
    {
#if ENABLE_INPUT_SYSTEM
        if (HasNewInputDevices())
        {
            Vector2 move = Vector2.zero;
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                move += gamepad.leftStick.ReadValue();
            }

            return Vector2.ClampMagnitude(move, 1f);
        }
#endif
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public static Vector2 GetLookDelta()
    {
#if ENABLE_INPUT_SYSTEM
        if (HasNewInputDevices())
        {
            Vector2 look = Vector2.zero;

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                look += mouse.delta.ReadValue() * 0.02f;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                look += gamepad.rightStick.ReadValue() * (GamepadLookSensitivity * Time.deltaTime);
            }

            return look;
        }
#endif
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public static bool GetInteractDown(KeyCode fallbackKey = KeyCode.E)
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        if ((kb != null && kb.eKey.wasPressedThisFrame) ||
            (gamepad != null && (gamepad.buttonSouth.wasPressedThisFrame || gamepad.rightTrigger.wasPressedThisFrame)))
        {
            return true;
        }
#endif
        return Input.GetKeyDown(fallbackKey);
    }

    public static bool GetCursorUnlockDown()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        if ((kb != null && kb.escapeKey.wasPressedThisFrame) ||
            (gamepad != null && gamepad.startButton.wasPressedThisFrame))
        {
            return true;
        }
#endif
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool GetCursorRelockDown()
    {
#if ENABLE_INPUT_SYSTEM
        Mouse mouse = Mouse.current;
        Gamepad gamepad = Gamepad.current;
        if ((mouse != null && mouse.leftButton.wasPressedThisFrame) ||
            (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))
        {
            return true;
        }
#endif
        return Input.GetMouseButtonDown(0);
    }

    public static bool GetRestartDown(KeyCode fallbackKey = KeyCode.R)
    {
        if (GetGamepadStartDown()) return true;
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame) return true;
#endif
        return Input.GetKeyDown(fallbackKey);
    }

#if ENABLE_INPUT_SYSTEM
    private static bool HasNewInputDevices()
    {
        return Keyboard.current != null || Mouse.current != null || Gamepad.current != null;
    }

#endif
}
