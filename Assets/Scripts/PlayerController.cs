using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    #region Events
    public event EventHandler OnDied;
    public event EventHandler OnMaximumBowTensionReached;
    #endregion

    #region Movement Booleans
    private bool isWalking;
    private bool isClimbing;
    private bool isPullingBow;
    private bool canMove;
    private bool isAlive;
    #endregion

    #region Other variables
    private float gravityScaleAtStart;
    #endregion

    #region Cache variables
    private Rigidbody2D rb;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private InputManager inputManager;
    private GameManager gameManager;
    #endregion


    #region Jumping
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpInputBufferTime = 0.1f;
    private float lastPressedJumpTime;
    private float lastOnGroundTime;
    private bool isJumping;
    #endregion
    #region Shooting
    private Vector2 shootingVelocity;
    [Header("Shooting")]
    [SerializeField] private Vector2 maxShootingVelocity = new Vector2(30f, 15f);
    [SerializeField] private Vector2 minShootingVelocity = new Vector2(15f, 5f);
    [SerializeField] private Vector2 tensionIncreaseRate = new Vector2(5f,5f);
    #endregion

    [Header("Run")]
    [Tooltip("The top speed the player can reach")]
    [SerializeField]private float runMaxSpeed = 10f;
    [Tooltip("Smoothing the behaviour, be careful about animations")]
    [SerializeField]private float runLerpAmount = 2f;
    [SerializeField]private float runAccelAmount = 7f;
    [SerializeField]private float runDeccelAmount = 7f;
    [Tooltip("Threshold for acceleration to set IsWalking to true")]
    [SerializeField] private float acclMovementThreshold = 0.01f;
    [Tooltip("Threshold for deceleration to set IsWalking to true")]
    [SerializeField] private float declMovementThreshold = 2f;


    [Header("Other Tunable Params")]
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private float knockBackSpeed = 10f;
    [SerializeField] private Vector2 deathKick = new Vector3(0f, 10f);


    [Space(5)]
    [Header("Transforms")]
    [SerializeField] private Transform gunTransform;


    [Space(5)]
    [Header("Masks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask climbingLayerMask;
    [SerializeField] private LayerMask enemiesLayerMask;
    [SerializeField] private LayerMask hazardLayerMask;


    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        footCol = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rb.gravityScale;
        canMove = true;
        isAlive = true;

        isWalking = false;
        isClimbing = false;

        //Shooting
        shootingVelocity = Vector2.zero;
        isPullingBow = false;

        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;
        isJumping = false;
    }

    private void Start()
    {
        inputManager = InputManager.GetInstance();
        gameManager = GameManager.GetInstance();
        inputManager.OnJump += OnJumpPerformed;
        inputManager.OnShootPerformed += OnShootPerformed;
        inputManager.OnShootCanceled += OnShootCanceled; 

    }
    // Update is called once per frame
    

    private void FixedUpdate()
    {
        if (!isAlive) return;
        HandleBowTension();
        Die();
        if (!canMove) return;
        Walk();
        Climb();
        FlipSprite();
        HandleJump();
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            if (footCol.IsTouchingLayers(groundLayerMask))
            {
                lastOnGroundTime = coyoteTime;
            }
        }

        lastPressedJumpTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;
        if(isJumping && rb.velocity.y < 0f)
        {
            isJumping = false;
        }


    }

    private void OnJumpPerformed(object sender, EventArgs e)
    {
        /*
         * I know this function may  seem very confusing. We check whether the player is grounded in HandleJump function.
         * If the player is not jumping and on ground, we set lastOnGroundTime to coyoteTime.
         * CanJump will check whether the player is in the air (isJumping), and other two variables.
         * 
         */
        lastPressedJumpTime = jumpInputBufferTime;
        if (!canMove || !isAlive || !CanJump()) return;
        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;
        float force = jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;
        
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);  
        isJumping = true;
        
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0f && !isJumping && lastPressedJumpTime > 0f;
    }

    public void Die()
    {
        if (bodyCol.IsTouchingLayers(enemiesLayerMask) || 
            bodyCol.IsTouchingLayers(hazardLayerMask) ||
            footCol.IsTouchingLayers(hazardLayerMask) ||
            footCol.IsTouchingLayers(enemiesLayerMask))
        {
            canMove = false;
            isAlive = false;
            OnDied?.Invoke(this, EventArgs.Empty);
            rb.velocity = deathKick;
            bodyCol.enabled = false;
            footCol.enabled = false;
            gameManager.ChangeStateTo(GameManager.GameState.GAMEOVER);

        }
    }

    private void Walk()
    {
        Vector2 movementVector = inputManager.GetMovementVector(); 

        float targetSpeed = movementVector.x * runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, runLerpAmount);

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;

        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if(accelRate == runAccelAmount)
        {
            if(Mathf.Abs(rb.velocity.x) > acclMovementThreshold)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > declMovementThreshold)
        {
            isWalking = true;

        }
        else
        {
            isWalking = false;
        }
    }

    private void Climb()
    {
        if (!bodyCol.IsTouchingLayers(climbingLayerMask))
        {
            isClimbing = false;

            rb.gravityScale = gravityScaleAtStart;
            return;
        }
        Vector2 movementVector = inputManager.GetMovementVector();
        Vector2 climbVelocity = new Vector2(rb.velocity.x, movementVector.y * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;

        if (HasVerticalSpeed() && bodyCol.IsTouchingLayers(climbingLayerMask))
        {
            isClimbing = true;

        }
        else
        {
            isClimbing = false;
        }
    }

    private void HandleBowTension()
    {
        if (!isPullingBow)
        {
            shootingVelocity = minShootingVelocity;
            return;
        }

        shootingVelocity.x += tensionIncreaseRate.x * Time.deltaTime;
        shootingVelocity.y += tensionIncreaseRate.y * Time.deltaTime;

        shootingVelocity.x = Mathf.Clamp(shootingVelocity.x, minShootingVelocity.x, maxShootingVelocity.x);
        shootingVelocity.y = Mathf.Clamp(shootingVelocity.y, minShootingVelocity.y, maxShootingVelocity.y);


        if (shootingVelocity.x >= maxShootingVelocity.x && shootingVelocity.y >= maxShootingVelocity.y)
        {
            OnMaximumBowTensionReached?.Invoke(this, EventArgs.Empty);
            isPullingBow = false;
            shootingVelocity = minShootingVelocity;
        }
    }


    private void OnShootCanceled(object sender, EventArgs e)
    {
        isPullingBow = false;
    }

    private void OnShootPerformed(object sender, EventArgs e)
    {
        if (!canMove || !isAlive) return;
        canMove = false;
        if (!isClimbing)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        isWalking = false;
        isClimbing = false;
        isPullingBow = true;
    }

    public void ShootArrow()
    {
        Transform arrow = Instantiate(gameManager.GetComponent<GameAssets>().GetArrow(), gunTransform.position, Quaternion.identity);
        arrow.gameObject.SetActive(true);
        float direction = Mathf.Sign(transform.localScale.x);
        arrow.GetComponent<Arrow>().SetVelocity(new Vector2((direction * shootingVelocity.x), shootingVelocity.y));
        //Add knocback
        Vector2 knockBackForce = new Vector2(-direction * knockBackSpeed, 0);
        rb.AddForce(knockBackForce, ForceMode2D.Impulse);

    }


    private void FlipSprite()
    {
        if (!IsWalking()) return;
        transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    private bool HasVerticalSpeed()
    {
        bool playerHasVerticalSpeed = Math.Abs(rb.velocity.y) > Mathf.Epsilon;
        return playerHasVerticalSpeed;
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsClimbing()
    {
        return isClimbing;
    }

    public void SetCanMove(bool newValue)
    {
        canMove = newValue;
    }





}
