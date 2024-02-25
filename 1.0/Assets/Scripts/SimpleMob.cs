using UnityEngine;

public class EnemyMob : MonoBehaviour
{
    public float speed = 1.0f; // Normal movement speed of the mob
    public float chaseSpeed = 2.0f; // Increased speed when chasing the player
    public int health = 3; // Health of the mob

    private GameObject target; // Target (player's kingdom or structures)
    private GameObject player; // Reference to the player

    void Start()
    {
        // Find the player's kingdom or a specific target. This is just a placeholder.
        target = GameObject.Find("Player");
        // Assuming the player has a tag of "Player"
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Move towards the target each frame
        MoveTowardsTarget();

        // Check for destruction
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the mob if health is depleted
        }
    }

    void MoveTowardsTarget()
    {
        float step = speed * Time.deltaTime; // Use normal speed by default

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // If the player is within a 5 unit radius, increase speed
        if (distanceToPlayer <= 5f)
        {
            step = chaseSpeed * Time.deltaTime; // Use chase speed
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    // Method to call when the mob takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    // OnCollisionEnter or OnTriggerEnter methods to handle collisions/damage
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Example: Take 1 damage if hit by a player's arrow
        if (collision.gameObject.CompareTag("PlayerArrow"))
        {
            TakeDamage(1);
        }
    }
}

