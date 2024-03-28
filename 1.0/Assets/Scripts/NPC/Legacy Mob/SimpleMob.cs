using UnityEngine;


public class EnemyMob : MonoBehaviour
{
    /*
    public float speed = 1.0f; // Normal movement speed of the mob
    public float chaseSpeed = 2.0f; // Increased speed when chasing the player
    public int health = 3; // Health of the mob
    public int attackDamage = 1; // Damage dealt by the mob
    public float attackRate = 1.0f; // How often the mob can attack in seconds
    public float attackRange = 1.5f; // Range within which the mob can attack

    private GameObject target; // Target (player's kingdom or structures)
    private GameObject player; // Reference to the player
    private float lastAttackTime = -1; // Time since last attack

    void Start()
    {
        target = GameObject.Find("Player");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        MoveTowardsTarget();

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        AttemptAttack();
    }

    void MoveTowardsTarget()
    {
        float step = speed * Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= 5f)
        {
            step = chaseSpeed * Time.deltaTime;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    void AttemptAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Check if the mob is within attack range and if enough time has passed since the last attack
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackRate)
        {
            // Make sure the player GameObject has a Damageable component attached
            Damageable playerDamageable = player.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                // Assuming your Damageable component's TakeDamage method accepts damage and knockback
                Vector2 knockback = new Vector2(0, 0); // Define knockback as needed
                playerDamageable.TakeDamage(attackDamage, knockback);
                lastAttackTime = Time.time; // Update the time of the last attack
            }
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerArrow"))
        {
            TakeDamage(1);
        }
    }
    */
}

