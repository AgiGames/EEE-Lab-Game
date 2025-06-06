using UnityEngine;
using UnityEngine.InputSystem;

[HelpURL("https://youtu.be/HkNVp04GOEI")]
public abstract class PressInputBase : MonoBehaviour
{
    protected InputAction m_PressAction;
    protected InputAction.CallbackContext? activeContext;

    protected virtual void Awake()
    {
        // Create a new input within the script.
        m_PressAction = new InputAction("touch", binding: "<Pointer>/press");

        // If touch is being started, call the OnPressBegan function.
        m_PressAction.started += ctx =>
        {
            activeContext = ctx; // Store active context
            if (ctx.control.device is Pointer device)
            {
                OnPressBegan(device.position.ReadValue());
            }
        };

        // If touch is being performed, call the OnPress function.
        m_PressAction.performed += ctx =>
        {
            activeContext = ctx; // Update active context
            if (ctx.control.device is Pointer device)
            {
                OnPress(device.position.ReadValue());
            }

            // Check for two touches.
            if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
            {
                var touch1 = Touchscreen.current.touches[0].position.ReadValue();
                var touch2 = Touchscreen.current.touches[1].position.ReadValue();
                OnTwoTouches(touch1, touch2);
            }
        };

        // If the existing touch is stopped or canceled, call the OnPressCancel function.
        m_PressAction.canceled += ctx =>
        {
            activeContext = null; // Clear active context.
            OnPressCancel();
        };
    }

    protected virtual void OnEnable()
    {
        m_PressAction.Enable();
    }

    protected virtual void OnDisable()
    {
        m_PressAction.Disable();
    }

    protected virtual void OnDestroy()
    {
        m_PressAction.Dispose();
    }

    protected virtual void OnPress(Vector3 position) { }

    protected virtual void OnPressBegan(Vector3 position) { }

    protected virtual void OnPressCancel() { }

    /// <summary>
    /// Called when two touches are detected simultaneously.
    /// </summary>
    /// <param name="touch1">Position of the first touch.</param>
    /// <param name="touch2">Position of the second touch.</param>
    protected virtual void OnTwoTouches(Vector2 touch1, Vector2 touch2) { }
}