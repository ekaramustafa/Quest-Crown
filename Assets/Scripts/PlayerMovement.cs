using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private const string IS_WALKING = "IsWalking";
    private const string IS_CLIMBING = "IsClimbing";

    private float gravityScaleAtStart;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;


    [Header("Tunable Params")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [Header("Masks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask climbingLayerMask;
    
    
    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        gravityScaleAtStart = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        FlipSprite();
        Climb();
    }

    private void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && col.IsTouchingLayers(groundLayerMask))
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

    private void Climb()
    {
        if (!col.IsTouchingLayers(climbingLayerMask))
        {
            animator.SetBool(IS_CLIMBING, false);
            rb.gravityScale = gravityScaleAtStart;
            return;
        }
        
        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;

        if (HasVerticalSpeed() && col.IsTouchingLayers(climbingLayerMask))
        {
            animator.SetBool(IS_CLIMBING, true);

        }
        else
        {
            animator.SetBool(IS_CLIMBING, false);
        }
    }



    private void FlipSprite()
    {
        if (!HasHorizantalSpeed()) return;
        transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    private bool HasVerticalSpeed()
    {
        bool playerHasVerticalSpeed = Math.Abs(rb.velocity.y) > Mathf.Epsilon;
        return playerHasVerticalSpeed;
    }
    private bool HasHorizantalSpeed()
    {
        bool playerHasHorizantalSpeed = Math.Abs(rb.velocity.x) > Mathf.Epsilon;
        return playerHasHorizantalSpeed;
    }


}
