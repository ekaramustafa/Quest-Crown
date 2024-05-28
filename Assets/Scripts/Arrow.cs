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
    [SerializeField]
    private float drag = 0.3f;

    private Vector2 velocity = Vector2.zero;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 velocity)
    {
        transform.localScale = new Vector3(Mathf.Sign(velocity.x),transform.localScale.y,transform.localScale.z);
        this.velocity = velocity;
    }

    private void Update()
    {
        velocity += gravity * Time.deltaTime;

        velocity -= velocity.sqrMagnitude * drag * velocity.normalized * Time.deltaTime;
        Vector3 amountToMove = new Vector3(velocity.x, velocity.y, 0f) * Time.deltaTime;
        transform.position += amountToMove;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((hitLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("HIT");
        }

        if ((destroyLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
