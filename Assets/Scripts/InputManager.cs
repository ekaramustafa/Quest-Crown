using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private static InputManager instance;

    PlayerInputActions inputActions;
    // Start is called before the first frame update


    public event EventHandler OnJump;
    public event EventHandler OnJoin;

    public static InputManager GetInstance()
    {
        return instance;
    }


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        inputActions = new PlayerInputActions();

        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += JumpPerformed;
        inputActions.Player.Join.performed += JoinPerformed;
    }

    private void OnDestroy()
    {
        inputActions.Player.Jump.performed -= JumpPerformed;
        inputActions.Player.Join.performed -= JoinPerformed;

        inputActions.Dispose();
    }

    private void JoinPerformed(InputAction.CallbackContext obj)
    {
        OnJoin?.Invoke(this, EventArgs.Empty);
    }

    private void JumpPerformed(InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }


 
}
