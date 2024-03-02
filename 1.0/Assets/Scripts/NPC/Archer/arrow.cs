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

        // Make the arrow stick to the object it collides with
        StickArrow(collision);
    }

    void StickArrow(Collision2D collision)
    {
        // Disable the Rigidbody2D to stop the arrow
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Optionally, make the arrow a child of the collided object so it moves with it
        // Remove this line if you don't want the arrow to become a child of what it hits
        transform.parent = collision.transform;

        // Disable this script (if it only controls flight) to stop further updates
        this.enabled = false;
    }
}
