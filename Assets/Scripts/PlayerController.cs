using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    public event EventHandler OnDied;
    public event EventHandler OnMaximumBowTensionReached;

    private bool isWalking;
    private bool isClimbing;

    private Vector2 shootingVelocity;
    private bool isPullingBow;

    private float gravityScaleAtStart;
    private bool canMove;
    private bool isAlive;

    private Rigidbody2D rb;
    private CapsuleCollider2D bodyCol;
    private BoxCollider2D footCol;

    private InputManager inputManager;
    private GameManager gameManager;

    [Header("Tunable Params")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private float knockBackSpeed = 10f;
    [SerializeField] private Vector2 deathKick = new Vector3(0f,10f);
    //TO-DO Add maximum shooting velocity and change the velocity depending on holding time
    [SerializeField] private Vector2 maxShootingVelocity = new Vector2(25f, 12f);
    [SerializeField] private Vector2 minShootingVelocity = new Vector2(10f, 5f);
    [SerializeField] private Vector2 tensionIncreaseRate = new Vector2(3f,3f);

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
    void Update()
    {
        if (!isAlive) return;
        HandleBowTension();
        Die();
        if (!canMove) return;
        Walk();
        Climb();
        FlipSprite();
    }


    private void OnJumpPerformed(object sender, EventArgs e)
    {
        if (!canMove || !isAlive) return;
        if (footCol.IsTouchingLayers(groundLayerMask))
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }


    private void Die()
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
        Vector2 playerVelocity = new Vector2(movementVector.x * walkSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        if (HasHorizantalSpeed())
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
        rb.velocity = new Vector2(-direction * knockBackSpeed, rb.velocity.y);

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
