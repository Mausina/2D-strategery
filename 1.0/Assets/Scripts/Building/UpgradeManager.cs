using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelIconContainers; // References to each level's icon container
    private int currentLevel = 0; // Tracks the current level
    private bool playerInRange = false;
    private int coinsPlaced = 0; // Tracks how many coins have been placed
    public ObjectUpgrade ObjectUpgrade;
    public TreeManager TreeManager;
    [SerializeField] private GameObject prefabCoin; // The coin prefab
    private List<GameObject> instantiatedCoins = new List<GameObject>(); // Tracks instantiated coins
    private float timeSinceLastCoin = 0f; // Timer for dropping coins
    private float coinDropDelay = 1f; // Delay before dropping coins
    public UpgradeBuildingAnimatio upgradeBuildingAnimation;
    private bool coinRecentlyPlaced = false;

    public bool CanPlaceCoin { get; private set; } = false;
    public bool isUpgrading;
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
        // If a coin has recently been placed, start or reset the timer
        if (coinRecentlyPlaced)
        {
            timeSinceLastCoin = 0f;
            coinRecentlyPlaced = false; // Reset the flag
            CanPlaceCoin = true; // Allow placing more coins
        }
        else if (playerInRange && coinsPlaced > 0)
        {
            // If the player is in range and has placed at least one coin, increment the timer
            timeSinceLastCoin += Time.deltaTime;

            // If the timer exceeds the delay, drop all coins and reset the timer
            if (timeSinceLastCoin >= coinDropDelay)
            {
                DropAllCoins();
                coinRecentlyPlaced = true; 
                CanPlaceCoin = false; // Prevent further coin placement until new interaction
            }
        }

        if (playerInRange)
        {
            UpdateIconVisibility(true); 
        }
    }

    public void AttemptPlaceOrDropCoin()
    {
        if (playerInRange && (!ObjectUpgrade?.IsMaxLevel ?? true) && (!TreeManager?.IsMaxLevel ?? true) && CanPlaceCoin)
        {
            PlaceCoin();
            coinRecentlyPlaced = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            CanPlaceCoin = true;
            UpdateIconVisibility(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CanPlaceCoin = false;
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


        currentLevel++; // Increment the level
        coinsPlaced = 0; // Reset coins placed
        instantiatedCoins.ForEach(Destroy); // Destroy all coins
        instantiatedCoins.Clear(); // Clear the list of instantiated coins


        if (ObjectUpgrade != null)
        {
            ObjectUpgrade.Upgrade();
        }
        else if (TreeManager != null)
        {
            TreeManager.TreeCutDown();
        }


        if (playerInRange) UpdateIconVisibility(true); // Update icon visibility if the player is still in range
    }

    private void UpdateIconVisibility(bool isVisible)
    {
        // Check if levelIconContainers is not null and has elements before trying to access them
        if (levelIconContainers != null && levelIconContainers.Count > 0)
        {
            // Hide all icons initially
            foreach (var container in levelIconContainers)
            {
                if (container != null) // Check if the container is not null
                {
                    container.SetActive(false);
                }
                else
                {
                    Debug.LogError("A container in levelIconContainers is null.");
                }
            }

            // Check if the current level is within the bounds of the list
            if (currentLevel >= 0 && currentLevel < levelIconContainers.Count)
            {
                // Also, check if levelIconContainers[currentLevel] is not null
                if (levelIconContainers[currentLevel] != null)
                {
                    // Check if the player is in range, the upgrade is not at max level, and upgrade is not completed
                    // Then show the icon for the current level
                    if (isVisible && (!ObjectUpgrade?.IsMaxLevel ?? true) && (!TreeManager?.IsMaxLevel ?? true))
                    {
                        levelIconContainers[currentLevel].SetActive(true);
                    }
                }
                else
                {
                    Debug.LogError($"The level icon container at index {currentLevel} is null.");
                }
            }
            else
            {
                Debug.LogError($"Current level index {currentLevel} is out of bounds for levelIconContainers list.");
            }
        }
        else
        {
            Debug.LogError("levelIconContainers list is null or empty.");
        }
    }


}



