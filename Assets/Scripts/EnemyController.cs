using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public event EventHandler OnDamageTaken;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private Vector3 previousPosition;

    [Header("Other params")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float knockBackDuration = 0.5f;

    [Header("Patrolling")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float patrollingRayDistance = 2f;
    [SerializeField] private LayerMask patrollingLayers;
    [SerializeField] private LayerMask avoidLayers;

    [Header("Transforms")]
    [SerializeField] private Transform groundDetection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        if (isKnockedBack) return;

        Patrol();
    }

    private void Patrol()
    {
        rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        FlipSpriteIfNeeded();
    }

    private void FlipSpriteIfNeeded()
    {
        bool needsFlip = !Physics2D.Raycast(groundDetection.position, Vector2.down, patrollingRayDistance, patrollingLayers) ||
                          Physics2D.Raycast(groundDetection.position, Vector2.down, patrollingRayDistance, avoidLayers);

        if (needsFlip)
        {
            movementSpeed = -movementSpeed;
            Flip();
        }
    }

    private void Flip()
    {
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
