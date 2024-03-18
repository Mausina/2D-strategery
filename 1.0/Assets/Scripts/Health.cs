using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health at the start
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Subtract damage from current health
        if (currentHealth <= 0)
        {
            Die(); // Handle death
        }
    }

    private void Die()
    {
        // Here you can add what happens when the object dies, e.g., play a death animation
        Destroy(gameObject); // Destroy the game object for simplicity
    }
}
