using System;
using UnityEngine;

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

    #region Gravity Variables
    private float DEFAULT_GRAVITY;
    private float gravityScaleAtStart;
    #endregion

    #region Cache Variables
    private Rigidbody2D rb;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private InputManager inputManager;
    private GameManager gameManager;
    #endregion

    #region Jumping Variables
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpInputBufferTime = 0.1f;
    [SerializeField] private float jumpCutGravityMult = 1.75f;
    [SerializeField] private float fallGravityMult = 2.5f;
    [SerializeField] private float jumpHangSpeedThreshold = 1f;
    [SerializeField] private float jumpHangGravityMult = 0.8f;

    private float lastPressedJumpTime;
    private float lastOnGroundTime;
    private bool isJumping;
    private bool isJumpCut;
    #endregion

    #region Shooting Variables
    private Vector2 shootingVelocity;
    [Header("Shooting")]
    [SerializeField] private Vector2 maxShootingVelocity = new Vector2(40f, 10f);
    [SerializeField] private Vector2 minShootingVelocity = new Vector2(25f, 5f);
    [SerializeField] private Vector2 tensionIncreaseRate = new Vector2(5f, 5f);
    #endregion

    #region Movement Variables
    [Header("Run")]
    [SerializeField] private float walkMaxSpeed = 10f;
    [SerializeField] private float walkLerpAmount = 2f;
    [SerializeField] private float walkAccelAmount = 7f;
    [SerializeField] private float walkDeccelAmount = 7f;
    [SerializeField] private float walkAcclMovementThreshold = 0.01f;
    [SerializeField] private float walkDeclMovementThreshold = 2f;


    [Space(5)]
    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 10f;
    private bool onLadder = false;
    #endregion

    [Space(5)]
    [Header("Die&Damage")]
    [SerializeField] private float knockBackSpeed = 10f;
    [SerializeField] private Vector2 deathKick = new Vector3(0f, 10f);

    #region Transforms and Layers
    [Space(5)]
    [Header("Transforms")]
    [SerializeField] private Transform gunTransform;

    [Space(5)]
    [Header("Masks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask climbingLayerMask;
    [SerializeField] private LayerMask enemiesLayerMask;
    [SerializeField] private LayerMask hazardLayerMask;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeComponents();
        InitializeVariables();
    }

    private void Start()
    {
        InitializeManagers();
        RegisterInputEvents();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        HandleBowTension();
        CheckForDeath();

        if (!canMove) return;

        HandleMovement();
        FlipSprite();
        HandleJump();
    }
    #endregion

    #region Initialization Methods
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        footCol = GetComponent<BoxCollider2D>();
    }

    private void InitializeVariables()
    {
        gravityScaleAtStart = rb.gravityScale;
        DEFAULT_GRAVITY = rb.gravityScale;
        canMove = true;
        isAlive = true;
        shootingVelocity = Vector2.zero;
        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;
        isJumping = false;
    }

    private void InitializeManagers()
    {
        inputManager = InputManager.GetInstance();
        gameManager = GameManager.GetInstance();
    }

    private void RegisterInputEvents()
    {
        inputManager.OnJumpStarted += OnJumpPerformed;
        inputManager.OnJumpCanceled += OnJumpCanceled;
        inputManager.OnShootPerformed += OnShootPerformed;
        inputManager.OnShootCanceled += OnShootCanceled;
    }
    #endregion

    #region Movement Methods
    private void HandleMovement()
    {
        Walk();
        Climb();
    }

    private void Walk()
    {
        Vector2 movementVector = inputManager.GetMovementVector();
        float targetSpeed = Mathf.Lerp(rb.velocity.x, movementVector.x * walkMaxSpeed, walkLerpAmount);
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? walkAccelAmount : walkDeccelAmount;
        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        isWalking = (accelRate == walkAccelAmount && Mathf.Abs(rb.velocity.x) > walkAcclMovementThreshold) ||
                    (Mathf.Abs(rb.velocity.x) > walkDeclMovementThreshold);
    }

    private void Climb()
    {
        if (!bodyCol.IsTouchingLayers(climbingLayerMask))
        {
            isClimbing = false;
            onLadder = false;
            rb.gravityScale = gravityScaleAtStart;
            return;
        }

        Vector2 movementVector = inputManager.GetMovementVector();
        rb.velocity = new Vector2(rb.velocity.x, movementVector.y * climbSpeed);
        rb.gravityScale = 0f;

        isClimbing = HasVerticalSpeed() && bodyCol.IsTouchingLayers(climbingLayerMask);
        onLadder = bodyCol.IsTouchingLayers(climbingLayerMask);
    }
    #endregion

    #region Jumping Methods
    private void HandleJump()
    {
        UpdateJumpTimers();
        PerformJumpCheck();

        ApplyJumpPhysics();
    }

    private void ApplyJumpPhysics()
    {
        if (onLadder)
        {
            SetGravityScale(0);
        }
        else if (isJumpCut)
        {
            SetGravityScale(DEFAULT_GRAVITY * jumpCutGravityMult);
        }
        else if (isJumping && Mathf.Abs(rb.velocity.y) < jumpHangSpeedThreshold)
        {
            SetGravityScale(DEFAULT_GRAVITY * jumpHangGravityMult);
        }
        else if (rb.velocity.y < 0)
        {
            SetGravityScale(DEFAULT_GRAVITY * fallGravityMult);
        }
        else
        {
            SetGravityScale(DEFAULT_GRAVITY);
        }
    }

    private void PerformJumpCheck()
    {
        if (lastPressedJumpTime > 0 && Time.time - lastOnGroundTime <= coyoteTime)
        {
            Jump();
            lastPressedJumpTime = 0;
        }
    }

    private void Jump()
    {
        float force = jumpForce - Mathf.Min(rb.velocity.y, 0);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        isJumping = true;
        isJumpCut = false;
    }
    private void UpdateJumpTimers()
    {
        if (footCol.IsTouchingLayers(groundLayerMask))
        {
            lastOnGroundTime = Time.time;
        }

        if (lastPressedJumpTime > 0)
        {
            lastPressedJumpTime -= Time.deltaTime;
        }
    }

    private void SetGravityScale(float value)
    {
        gravityScaleAtStart = value;
        rb.gravityScale = value;
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }


    #endregion

    #region Jump Event Handlers
    private void OnJumpPerformed(object sender, EventArgs e)
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    private void OnJumpCanceled(object sender, EventArgs e)
    {
        if (CanJumpCut())
        {
            isJumpCut = true;
        }
    }
    #endregion


    #region Shooting Methods
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
        rb.velocity = isClimbing ? Vector2.zero : new Vector2(0, rb.velocity.y);

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

        Vector2 knockBackForce = new Vector2(-direction * knockBackSpeed, 0);
        rb.AddForce(knockBackForce, ForceMode2D.Impulse);
    }
    #endregion

    #region Death Methods
    private void CheckForDeath()
    {
        if (IsTouchingLayers(bodyCol, enemiesLayerMask, hazardLayerMask) || IsTouchingLayers(footCol, hazardLayerMask))
        {
            Die();
        }
    }

    private void Die()
    {
        canMove = false;
        isAlive = false;
        OnDied?.Invoke(this, EventArgs.Empty);
        rb.velocity = deathKick;
        bodyCol.enabled = false;
        footCol.enabled = false;
        gameManager.ChangeStateTo(GameManager.GameState.GAMEOVER);
    }
    #endregion

    #region Helper Methods
    private bool IsTouchingLayers(Collider2D collider, LayerMask layerMask1, LayerMask layerMask2)
    {
        return collider.IsTouchingLayers(layerMask1) || collider.IsTouchingLayers(layerMask2);
    }

    private bool IsTouchingLayers(Collider2D collider, LayerMask layerMask1)
    {
        return collider.IsTouchingLayers(layerMask1);
    }
    private void FlipSprite()
    {
        if (!IsWalking()) return;
        transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    private bool HasVerticalSpeed()
    {
        return Math.Abs(rb.velocity.y) > Mathf.Epsilon;
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
    #endregion
}
