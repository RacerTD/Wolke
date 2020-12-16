using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbility : Ability
{
    protected InputAction.CallbackContext currentInput;

    /// <summary>
    /// Gets the input from the player
    /// </summary>
    public void AbiltiyPlayerInput(InputAction.CallbackContext context)
    {
        currentInput = context;
    }

    /// <summary>
    /// Happens when the input device changes
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceChanged(PlayerInput input)
    {

    }

    /// <summary>
    /// Happens when the current input device disconnects
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceLost(PlayerInput input)
    {

    }

    /// <summary>
    /// Happens when the current input device is reconnected
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnInputDeviceReconnected(PlayerInput input)
    {

    }
}
