using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private bool isMovingRight;

    [Header("Other params")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float knockBackDuration = 0.5f;

    [Space(5)]
    [Header("Patrolling")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float patrollingRayDistance = 2f;
    [SerializeField] private LayerMask patrollingLayers;
    [SerializeField] private LayerMask avoidLayers;

    [Space(5)]
    [Header("Transforms")]
    [SerializeField] private Transform patrolGroundDetection;

    public event EventHandler OnDamageTaken;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isMovingRight = true;
    }

    private void Update()
    {
        if (isKnockedBack) return;

        Patrol();
    }

    private void Patrol()
    {
        if (isMovingRight)
        {
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }

        FlipSpriteIfNeeded();
    }

    private void FlipSpriteIfNeeded()
    {
        RaycastHit2D patrolInfo = Physics2D.Raycast(patrolGroundDetection.position, Vector2.down, patrollingRayDistance,patrollingLayers);
        RaycastHit2D groundInfo = Physics2D.Raycast(patrolGroundDetection.position, Vector2.down, patrollingRayDistance, avoidLayers);
        if (patrolInfo.collider == null || groundInfo.collider != null)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void TakeDamage(float damage, Vector2 knockBack)
    {
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(ApplyKnockBack(knockBack));
        }
    }

    private IEnumerator ApplyKnockBack(Vector2 knockBack)
    {
        isKnockedBack = true;
        rb.velocity = knockBack;
        yield return new WaitForSeconds(knockBackDuration);
        isKnockedBack = false;
    }
}
