using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private const float GRAVITY = -9.81f;
    private static Vector2 gravity = Vector2.up * GRAVITY;


    [Header("LayerMasks")]
    [SerializeField] private LayerMask destroyLayer;
    [SerializeField] private LayerMask hitLayer;

    [Header("Tunable parameters")]
    [SerializeField] private float drag = 0.3f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private Vector2 knockBack = new Vector2(5f,5f);

    private Vector2 velocity = Vector2.zero;
    private float direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = 1f;
    }

    public void SetVelocity(Vector2 velocity)
    {
        direction = Mathf.Sign(velocity.x);
        transform.localScale = new Vector3(direction,transform.localScale.y,transform.localScale.z);
        this.velocity = velocity;
    }

    private void Update()
    {
        velocity += gravity * Time.deltaTime;

        velocity -= velocity.sqrMagnitude * drag * velocity.normalized * Time.deltaTime;
        Vector3 amountToMove = new Vector3(velocity.x, velocity.y, 0f) * Time.deltaTime;
        transform.position += amountToMove;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (velocity.x < 0) angle += 180;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((hitLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage,new Vector2(knockBack.x * direction, knockBack.y));
            Destroy(gameObject);
        }

        if ((destroyLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
