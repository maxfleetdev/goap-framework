using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player/Input Data")]
public class InputData : ScriptableObject, GameControls.IPlayerActions
{
    private GameControls gameControls = null;
    private bool isInitialised = false;

    // Null Actions
    public event Action OnPickupEvent;
    public event Action OnCrouchEvent;
    public event Action OnPlaceEvent;
    public event Action OnRemoveEvent;
    public event Action OnDropEvent;
    public event Action OnBuildModeEvent;
    public event Action OnRemoveObjectEvent;

    // Bool Actions
    public event Action<bool> OnJumpEvent;
    public event Action<bool> OnSprintEvent;

    // Float Actions
    public event Action<float> OnObjectChangeEvent;
    public event Action<float> OnObjectRotateEvent;

    // Vector Actions
    public event Action<Vector2> OnLookEvent;
    public event Action<Vector2> OnMoveEvent;

    public void Initialise()
    {
        if (isInitialised)
        {
            return;
        }

        gameControls = new GameControls();
        gameControls.Player.SetCallbacks(this);
        gameControls.Player.Enable();
        isInitialised = true;
    }

    public void Cleanup()
    {
        gameControls.Player.RemoveCallbacks(this);
        gameControls.Player.Disable();
        isInitialised = false;
    }

    #region Player Actions

    // PLAYER MOVEMENT INPUT

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnCrouchEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnJumpEvent?.Invoke(true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnJumpEvent?.Invoke(false);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnSprintEvent?.Invoke(true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnSprintEvent?.Invoke(false);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        OnLookEvent?.Invoke(
            context.ReadValue<Vector2>() == Vector2.zero ?
            Vector2.zero : context.ReadValue<Vector2>()
            );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(
            context.ReadValue<Vector2>() == Vector2.zero ? 
            Vector2.zero : context.ReadValue<Vector2>()
            );
    }

    // OBJECT INTERACTION INPUT

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnDropEvent?.Invoke();
        }
    }

    public void OnPlaceItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnPlaceEvent?.Invoke();
        }
    }

    public void OnRemoveItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnRemoveEvent?.Invoke();
        }
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnPickupEvent?.Invoke();
        }
    }

    // BUILDING INPUT

    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnBuildModeEvent?.Invoke();
        }
    }

    public void OnObjectChange(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnObjectChangeEvent?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnRotateObject(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnObjectRotateEvent?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnRemoveObject(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnRemoveObjectEvent?.Invoke();
        }
    }

    #endregion
}