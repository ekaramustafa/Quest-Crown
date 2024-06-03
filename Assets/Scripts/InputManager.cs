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


    public event EventHandler OnJumpStarted;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnJoin;
    public event EventHandler OnShootPerformed;
    public event EventHandler OnShootCanceled;


    [SerializeField] private Transform playerPrefab;

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
        inputActions.Player.Jump.canceled += JumpCanceled;
        inputActions.Player.Join.performed += JoinPerformed;
        inputActions.Player.Fire.performed += FirePerformed;
        inputActions.Player.Fire.canceled += FireCanceled;
       


    }

    private void JumpCanceled(InputAction.CallbackContext obj)
    {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void FireCanceled(InputAction.CallbackContext obj)
    {
        OnShootCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void FirePerformed(InputAction.CallbackContext obj)
    {
        OnShootPerformed?.Invoke(this,EventArgs.Empty);
        
    }

    private void OnDestroy()
    {
        inputActions.Player.Jump.performed -= JumpPerformed;
        inputActions.Player.Join.performed -= JoinPerformed;
        inputActions.Player.Fire.performed -= FirePerformed;
        inputActions.Player.Fire.performed -= FireCanceled;
        inputActions.Dispose();
    }

    private void JoinPerformed(InputAction.CallbackContext obj)
    {
        OnJoin?.Invoke(this, EventArgs.Empty);
        //Transform newPlayer = PlayerInput.Instantiate(playerPrefab);
        //newPlayer.gameObject.SetActive(true);
    }

    private void JumpPerformed(InputAction.CallbackContext obj)
    {
        OnJumpStarted?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector2 GetMouseDelta()
    {
        Vector2 mouseVector = inputActions.Player.Look.ReadValue<Vector2>();
        return mouseVector;
    }

    public Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        return mousePosition;
    }


 
}
