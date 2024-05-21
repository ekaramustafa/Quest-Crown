using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Animation string variables 
    private const string IS_WALKING = "IsWalking";
    private const string IS_CLIMBING = "IsClimbing";
    private const string DYING = "Dying";

    private float gravityScaleAtStart;
    private bool isAlive;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private PlayerInput playerInput;

    [Header("Tunable Params")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private Vector2 deathKick = new Vector3(10f,10f);

    [Header("Masks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask climbingLayerMask;
    [SerializeField] private LayerMask enemiesLayerMask;
    [SerializeField] private LayerMask hazardLayerMask;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        footCol = GetComponent<BoxCollider2D>();
        playerInput = GetComponent<PlayerInput>();
        gravityScaleAtStart = rb.gravityScale;
        isAlive = true;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;
        Walk();
        FlipSprite();
        Climb();
        Die();
    }

    private void Die()
    {
        if (bodyCol.IsTouchingLayers(enemiesLayerMask))
        {
            isAlive = false;
            animator.SetTrigger(DYING);
            rb.velocity = deathKick;
            bodyCol.enabled = false;
            footCol.enabled = false;
            //rb.bodyType = RigidbodyType2D.Static;
        }
        if (bodyCol.IsTouchingLayers(hazardLayerMask) || footCol.IsTouchingLayers(hazardLayerMask))
        {
            isAlive = false;
            animator.SetTrigger(DYING);
            rb.velocity = new Vector2(0f,deathKick.y);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump(InputValue inputValue)
    {
        if (!isAlive) return;
        if (inputValue.isPressed && footCol.IsTouchingLayers(groundLayerMask))
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void OnJoin(InputValue inputValue)
    {
        GameManager.GetInstance().PlayerCount++;
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
        if (!bodyCol.IsTouchingLayers(climbingLayerMask))
        {
            animator.SetBool(IS_CLIMBING, false);
            rb.gravityScale = gravityScaleAtStart;
            return;
        }
        
        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;

        if (HasVerticalSpeed() && bodyCol.IsTouchingLayers(climbingLayerMask))
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
