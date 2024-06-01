using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isKnockedBack = false;
    [Header("Tunable Parameters")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float knockBackDuration = 0.5f;



    public event EventHandler OnTakenDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        if (!isKnockedBack)
        {
            rb.velocity = new Vector2(movementSpeed, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        movementSpeed = -movementSpeed;
        FlipSprite();
    }

    private void FlipSprite()
    {
        float sign = -Mathf.Sign(rb.velocity.x);
        transform.localScale = new Vector2(sign, 1f);

    }

    public void TakeDamage(float damage, Vector2 knockBack)
    {
        OnTakenDamage?.Invoke(this, EventArgs.Empty);
        health -= damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
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
