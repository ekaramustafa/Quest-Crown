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

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private InputManager inputManager;

    [Header("Tunable Params")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private Vector2 deathKick = new Vector3(0f,10f);

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
        gravityScaleAtStart = rb.gravityScale;
        isAlive = true;
       
    }

    private void Start()
    {
        inputManager = InputManager.GetInstance();
        inputManager.OnJump += OnJumpPerformed;
    }

    private void OnJumpPerformed(object sender, EventArgs e)
    {
        if (!isAlive) return;
        if (footCol.IsTouchingLayers(groundLayerMask))
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
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
        if (bodyCol.IsTouchingLayers(enemiesLayerMask) || 
            bodyCol.IsTouchingLayers(hazardLayerMask) ||
            footCol.IsTouchingLayers(hazardLayerMask))
        {
            isAlive = false;
            animator.SetTrigger(DYING);
            rb.velocity = deathKick;
            bodyCol.enabled = false;
            footCol.enabled = false;
        }
    }

    private void Walk()
    {
        Vector2 movementVector = inputManager.GetMovementVector(); 
        Vector2 playerVelocity = new Vector2(movementVector.x * walkSpeed, rb.velocity.y);
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
        Vector2 movementVector = inputManager.GetMovementVector();
        Vector2 climbVelocity = new Vector2(rb.velocity.x, movementVector.y * climbSpeed);
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
