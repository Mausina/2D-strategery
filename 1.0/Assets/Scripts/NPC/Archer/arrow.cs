using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    public float destroyAfterSeconds = 5f; // Time to auto-destroy the arrow

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyAfterSeconds); // Destroy the arrow after some time to clean up
    }

    void Update()
    {
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Example collision logic
        Debug.Log($"Hit {collision.gameObject.name}");
        Destroy(gameObject); // Destroy the arrow on collision
    }
}
