using UnityEngine;

public class Arrow : MonoBehaviour
{
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
            // Calculate the rotation angle of the arrow based on its velocity
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            // Apply the rotation to the arrow
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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
