using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    private GameObject player;
    private float playerNearbyTime = 0f;
    private bool playerIsNearby = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player GameObject
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= 4.5f) // If the player is within 3 units
            {
                if (!playerIsNearby)
                {
                    playerIsNearby = true; // Player has just come within range
                    playerNearbyTime = 0f; // Reset the timer
                }

                playerNearbyTime += Time.deltaTime;
                if (playerNearbyTime >= 1f) // 2 seconds have passed
                {
                    CoinManager.Instance.AddCoins(value);
                    Destroy(gameObject); // Destroy the coin object
                }
            }
            else
            {
                playerIsNearby = false;
                playerNearbyTime = 0f; // Reset the timer as the player is no longer nearby
            }
        }
    }
}
