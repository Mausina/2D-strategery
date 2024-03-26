using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 25; // The amount of damage the arrow does
    private Rigidbody2D rb;
    public float destroyAfterSeconds = 5f; // Time to auto-destroy the arrow
    private bool isFlying = true; // To check if the arrow is flying

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyAfterSeconds); // Destroy the arrow after some time to clean up
    }

    void Update()
    {
        if (isFlying && rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            // If your sprite is, for example, drawn at a 45-degree angle in the sprite image, subtract 45 degrees
            angle -= 45; // Adjust this value as needed for your sprite
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Keep the debug line to check alignment
            Debug.DrawLine(transform.position, transform.position + new Vector3(rb.velocity.x, rb.velocity.y, 0), Color.red);
        }
    }




    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has a Health component
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            // If so, apply damage
            health.TakeDamage(damage);
        }

        // Once the arrow collides, make it stick and stop rotating it
        isFlying = false; // The arrow is no longer flying
        StickArrow(collision);
    }

    void StickArrow(Collision2D collision)
    {
        // Disable the Rigidbody2D to stop the arrow
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Make the arrow a child of the collided object
        transform.parent = collision.transform;

        // Disable this script to stop further updates
        this.enabled = false;
    }
}
