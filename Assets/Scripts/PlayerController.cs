using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Animation string variables 
    private const string IS_WALKING = "IsWalking";
    private const string IS_CLIMBING = "IsClimbing";
    private const string DYING = "Dying";
    private const string SHOOTING_ATTEMPT = "ShootingAttempt";
    private const string SHOOTING_WAITING = "ShootingWaiting";
    private const string SHOOTING_RELEASING = "ShootingReleasing";

    private float gravityScaleAtStart;
    private bool canMove;

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private InputManager inputManager;
    private GameManager gameManager;

    [Header("Tunable Params")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private float knockBackSpeed = 5f;
    [SerializeField] private Vector2 deathKick = new Vector3(0f,10f);
    [SerializeField] private Vector2 shootingVelocity = new Vector2(20f, 10f);

    [Header("Masks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask climbingLayerMask;
    [SerializeField] private LayerMask enemiesLayerMask;
    [SerializeField] private LayerMask hazardLayerMask;

    [Header("Transforms")]
    [SerializeField] private Transform gunTransform;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        footCol = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb.gravityScale;
        canMove = true;
       
    }

    private void Start()
    {
        inputManager = InputManager.GetInstance();
        gameManager = GameManager.GetInstance();
        inputManager.OnJump += OnJumpPerformed;
        inputManager.OnShootPerformed += OnShootPerformed;
        inputManager.OnShootCanceled += OnShootCanceled;

    }

    private void OnJumpPerformed(object sender, EventArgs e)
    {
        if (!canMove) return;
        if (footCol.IsTouchingLayers(groundLayerMask))
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        Walk();
        FlipSprite();
        Climb();
        Die();
    }

    private void Die()
    {
        if (bodyCol.IsTouchingLayers(enemiesLayerMask) || 
            bodyCol.IsTouchingLayers(hazardLayerMask) ||
            footCol.IsTouchingLayers(hazardLayerMask) ||
            footCol.IsTouchingLayers(enemiesLayerMask))
        {
            canMove = false;
            animator.SetTrigger(DYING);
            rb.velocity = deathKick;
            bodyCol.enabled = false;
            footCol.enabled = false;
            gameManager.ChangeStateTo(GameManager.GameState.GAMEOVER);
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


    /// <summary>
    /// These two functions are going to provide the mechanism of freezing the player 
    /// when it is shooting to avoid any animation glitches (e.g. legs are not moving while shooting and walking)
    /// </summary>

    private void OnShootPerformed(object sender, EventArgs e)
    {
        if (!canMove) return;
        animator.SetBool(SHOOTING_ATTEMPT, true);
    }

    //animation event reference
    private void AtShootingStarted()
    {
        rb.velocity = Vector2.zero;
        canMove = false;
    }

    //animation event reference
    private void AtShootingAttemptingFinished()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool(SHOOTING_WAITING,true);
    }


    //animation event reference
    private void AtShootingMoment()
    {
        Transform arrow = Instantiate(gameManager.GetComponent<GameAssets>().GetArrow(),gunTransform.position,Quaternion.identity);
        arrow.gameObject.SetActive(true);
        float direction = Mathf.Sign(transform.localScale.x);
        arrow.GetComponent<Arrow>().SetVelocity(new Vector2((direction * shootingVelocity.x), shootingVelocity.y));
        //Add knocback
        rb.velocity = Vector2.zero;
        canMove = false;
        rb.velocity = new Vector2(-direction * knockBackSpeed, 0f);    
    }
    private void OnShootCanceled(object sender, EventArgs e)
    {
        animator.SetBool(SHOOTING_RELEASING, true);
        animator.SetBool(SHOOTING_ATTEMPT, false);
        animator.SetBool(SHOOTING_WAITING, false);
    }

    //animation event reference
    private void AtShootingFinished()
    {
        rb.velocity = Vector2.zero;
        canMove = true;
        animator.SetBool(SHOOTING_RELEASING, false);
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
    public bool HasHorizantalSpeed()
    {
        bool playerHasHorizantalSpeed = Math.Abs(rb.velocity.x) > Mathf.Epsilon;
        return playerHasHorizantalSpeed;
    }

  


}
