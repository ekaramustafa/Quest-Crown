using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private const string IS_WALKING = "IsWalking";
    private const string IS_CLIMBING = "IsClimbing";

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        FlipSprite();
    }

    private void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && rb.velocity.y == 0)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void Walk()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        if (HasHorizantalSpeed())
        {
            animator.SetBool(IS_WALKING, true);
        }
        else
        {
            animator.SetBool(IS_WALKING, false);
        }
    }

    private void Jump()
    {

    }

    private void FlipSprite()
    {
        if (!HasHorizantalSpeed()) return;
        transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }


    private bool HasHorizantalSpeed()
    {
        bool playerHasHorizantalSpeed = Math.Abs(rb.velocity.x) > Mathf.Epsilon;
        return playerHasHorizantalSpeed;
    }


}
