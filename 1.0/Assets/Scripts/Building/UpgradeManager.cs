using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelIconContainers; // References to each level's icon container
    private int currentLevel = 0; // Tracks the current level
    private bool playerInRange = false;
    private int coinsPlaced = 0; // Tracks how many coins have been placed
    public Wall wall;
    [SerializeField] private GameObject prefabCoin; // The coin prefab
    private List<GameObject> instantiatedCoins = new List<GameObject>(); // Tracks instantiated coins
    private float timeSinceLastCoin = 0f; // Timer for dropping coins
    private float coinDropDelay = 1f; // Delay before dropping coins

    void Start()
    {
        // Initially disable all level icon containers
        foreach (var container in levelIconContainers)
        {
            container.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !wall.IsMaxLevel)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlaceCoin();
                timeSinceLastCoin = 0f;
            }

            if (coinsPlaced > 0)
            {
                timeSinceLastCoin += Time.deltaTime;
                if (timeSinceLastCoin >= coinDropDelay)
                {
                    DropAllCoins();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UpdateIconVisibility(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            DropAllCoins();
            UpdateIconVisibility(false);
        }
    }


    private void DropAllCoins()
    {
        foreach (var coin in instantiatedCoins)
        {
            coin.GetComponent<Rigidbody2D>().isKinematic = false;
            coin.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1);
        }

        instantiatedCoins.Clear();
        coinsPlaced = 0;
        timeSinceLastCoin = 0f;
    }

    private void PlaceCoin()
    {
        // Ensure the player can afford to place a coin
        if (CoinManager.Instance.CanAfford(1))
        {
            // Deduct a coin from the player's balance
            CoinManager.Instance.SpendCoins(1);

            // Check if there's a spot for the new coin
            if (levelIconContainers[currentLevel].transform.childCount > coinsPlaced)
            {
                // Get the position where the new coin should be placed
                Transform coinPosition = levelIconContainers[currentLevel].transform.GetChild(coinsPlaced);
                // Instantiate the coin at the specified position
                GameObject coin = Instantiate(prefabCoin, coinPosition.position, Quaternion.identity);
                coin.GetComponent<Rigidbody2D>().isKinematic = true; // Make the coin static
                instantiatedCoins.Add(coin); // Keep track of the coin
                coinsPlaced++; // Increment the number of coins placed
            }

            // Check if the current level is fully upgraded
            if (coinsPlaced >= levelIconContainers[currentLevel].transform.childCount)
            {
                CompleteUpgrade(); // Trigger the upgrade completion logic
            }
        }
        else
        {
            Debug.Log("Not enough coins to place!");
        }
    }


    private void CompleteUpgrade()
    {
        Debug.Log("Upgrade complete!");
        wall.UpgradeWall();
        currentLevel++; // Increment the level
        coinsPlaced = 0; // Reset coins placed
        instantiatedCoins.ForEach(Destroy); // Destroy all coins
        instantiatedCoins.Clear(); // Clear the list of instantiated coins

        if (playerInRange) UpdateIconVisibility(true); // Update icon visibility if the player is still in range
    }

    private void UpdateIconVisibility(bool isVisible)
    {
        foreach (var container in levelIconContainers)
        {
            container.SetActive(false);
        }

        if (isVisible && currentLevel < levelIconContainers.Count)
        {
            levelIconContainers[currentLevel].SetActive(true);
        }
    }
}









