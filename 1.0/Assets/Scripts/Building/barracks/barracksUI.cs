using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barracksUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> queueIconContainers; // References to each level's icon container
    [SerializeField] private GameObject prefabCoin;
    private int currentLevel = 0;
    private List<GameObject> instantiatedCoins = new List<GameObject>();
    public barracksManager barracksManager;
    private bool playerInRange = false;
    private int coinsPlaced = 0;
    private float timeSinceLastCoin = 0f;
    private float coinDropDelay = 1f;
    private bool coinRecentlyPlaced = false;

    public bool CanPlaceCoin { get; private set; } = false;

    void Start()
    {
        // Initially disable all level icon containers
        foreach (var container in queueIconContainers)
        {
            container.SetActive(false);
        }

    }
    

    void Update()
    {
        Test();
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
            UpdateIconVisibility(barracksManager.CanAddToQueue());
        }
    }

    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AttemptPlaceOrDropCoin();
        }
    }

    public void AttemptPlaceOrDropCoin()
    {
        if (playerInRange && barracksManager.CanAddToQueue() && CanPlaceCoin)
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
            if (queueIconContainers[currentLevel].transform.childCount > coinsPlaced)
            {
                // Get the position where the new coin should be placed
                Transform coinPosition = queueIconContainers[currentLevel].transform.GetChild(coinsPlaced);
                // Instantiate the coin at the specified position
                GameObject coin = Instantiate(prefabCoin, coinPosition.position, Quaternion.identity);
                coin.GetComponent<Rigidbody2D>().isKinematic = true; // Make the coin static
                instantiatedCoins.Add(coin); // Keep track of the coin
                coinsPlaced++; // Increment the number of coins placed
            }

            // Check if the current level is fully upgraded
            if (coinsPlaced >= queueIconContainers[currentLevel].transform.childCount)
            {
                CompleteAddToQueue(); // Trigger the upgrade completion logic
            }
        }
        else
        {
            Debug.Log("Not enough coins to place!");
        }
    }




    private void CompleteAddToQueue()
    {
        // Implement completion logic
        barracksManager.AddToQueue();
        foreach (var coin in instantiatedCoins)
        {
            Destroy(coin); // Assuming you want to destroy the coin after completion
        }
        instantiatedCoins.Clear(); // Clear the list of instantiated coins
        coinsPlaced = 0; // Reset the number of coins placed
    }


    private void UpdateIconVisibility(bool isVisible)
    {
        foreach (var container in queueIconContainers)
        {
            container.SetActive(false);
        }
        if (isVisible && barracksManager.CanAddToQueue())
        {
            foreach (var container in queueIconContainers)
            {
                container.SetActive(true); 
            }
        }
    }
}