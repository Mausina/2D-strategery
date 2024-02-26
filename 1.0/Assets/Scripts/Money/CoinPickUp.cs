using UnityEngine;
using System.Collections;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    private GameObject player;
    private float playerNearbyTime = 0f;
    private bool playerIsNearby = false;
    private bool isBeingCollected = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player != null && !isBeingCollected)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= 4.5f) // If the player is within 4.5 units
            {
                if (!playerIsNearby)
                {
                    playerIsNearby = true; // Player has just come within range
                    playerNearbyTime = 0f; // Reset the timer
                }

                playerNearbyTime += Time.deltaTime;
                if (playerNearbyTime >= 0.75f) // 1 second has passed
                {
                    StartCoroutine(CollectCoin());
                }
            }
            else
            {
                playerIsNearby = false;
                playerNearbyTime = 0f; // Reset the timer as the player is no longer nearby
            }
        }
    }

    private IEnumerator CollectCoin()
    {
        //Debug.Log("CollectCoin coroutine started.");
        isBeingCollected = true; // The coin is being collected, so set the flag

        // Optional: Add a sound effect or particle effect here

        float duration = 0.2f; // The duration of the movement towards the player
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, player.transform.position, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CoinManager.Instance.AddCoins(value); // Add the coin to the player's total
        Destroy(gameObject); // Destroy the coin object
    }
}
