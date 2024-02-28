using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    [SerializeField] private int upgradeCost = 3; // Total coins needed for an upgrade
    private bool playerInRange = false;
    private int coinsPlaced = 0;
    public Wall wall;
    [SerializeField] private GameObject prefabCoin;
    [SerializeField] private List<GameObject> coinIcons; // GameObjects of the icons which will be activated/deactivated
    private List<GameObject> instantiatedCoins = new List<GameObject>(); // To keep track of the instantiated coins
    private float timeSinceLastCoin = 0f;
    private float coinDropDelay = 1f; // Time in seconds after which coins drop if no new coins are added

    void Start()
    {
        visualCue.SetActive(false);
        // Initially, all coin icons are displayed
        foreach (var icon in coinIcons)
        {
            icon.SetActive(true);
        }
    }



    void Update()
    {
        visualCue.SetActive(playerInRange);
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Assuming 'E' is the key to place a coin
            {
                PlaceCoin();
                timeSinceLastCoin = 0f; // Reset the timer each time a coin is placed
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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // If the player leaves the trigger, drop all coins
            DropAllCoins();
        }
    }


    private void DropAllCoins()
    {
        foreach (var coin in instantiatedCoins)
        {
            coin.GetComponent<Rigidbody2D>().isKinematic = false; // Unfreeze the coin
            coin.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1); // Add some initial velocity to drop it
        }

        // Clear the list of instantiated coins and reset the coin icons
        instantiatedCoins.Clear();
        foreach (var icon in coinIcons)
        {
            icon.SetActive(true);
        }

        coinsPlaced = 0;
        timeSinceLastCoin = 0f;
    }

    private void PlaceCoin()
    {
        if (coinsPlaced < upgradeCost && CoinManager.Instance.CanAfford(1)) // Check if player can afford to place another coin
        {
            CoinManager.Instance.SpendCoins(1); // Subtract one coin from balance
            GameObject coin = Instantiate(prefabCoin, coinIcons[coinsPlaced].transform.position, Quaternion.identity);
            coin.GetComponent<Rigidbody2D>().isKinematic = true; // Freeze the coin in place
            instantiatedCoins.Add(coin); // Add the coin to the list
            coinIcons[coinsPlaced].SetActive(false); // Hide the icon since the coin is now representing it

            coinsPlaced++;

            if (coinsPlaced == upgradeCost)
            {
                CompleteUpgrade();
            }
        }
    }

    private void CompleteUpgrade()
    {
        Debug.Log("Upgrade complete!");
        wall.UpgradeWall(); // Upgrade the wall

        // Check if the wall has reached its maximum level after the upgrade
        if (wall.IsMaxLevel)
        {
            // Hide all coin icons because no more upgrades are possible
            foreach (var icon in coinIcons)
            {
                icon.SetActive(false);
            }
        }
        else
        {
            // If not at max level, ensure icons are visible for the next upgrade
            foreach (var icon in coinIcons)
            {
                icon.SetActive(true);
            }
        }

        // Destroy the physical coin objects that have been placed
        foreach (var coin in instantiatedCoins)
        {
            Destroy(coin); // This removes the coin GameObject from the scene
        }
        instantiatedCoins.Clear(); // Clear the list since all coins are destroyed

        coinsPlaced = 0;
    }


}

















