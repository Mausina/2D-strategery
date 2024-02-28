using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<int> upgradeCosts;
    [SerializeField] private List<GameObject> levelIconContainers;
    private int currentLevel = 0;
    private bool playerInRange = false;
    private int coinsPlaced = 0;
    public Wall wall;
    [SerializeField] private GameObject prefabCoin;
    private List<GameObject> instantiatedCoins = new List<GameObject>();
    private float timeSinceLastCoin = 0f;
    private float coinDropDelay = 1f;

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
            // Activate only the current level's icon container
            UpdateIconVisibility(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            DropAllCoins(); // Drop all coins if the player leaves the trigger area
            UpdateIconVisibility(false); // Optionally hide icons when player exits
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
        int upgradeCost = currentLevel < upgradeCosts.Count ? upgradeCosts[currentLevel] : upgradeCosts[upgradeCosts.Count - 1];

        if (coinsPlaced < upgradeCost && CoinManager.Instance.CanAfford(1))
        {
            CoinManager.Instance.SpendCoins(1);
            if (levelIconContainers[currentLevel].transform.childCount > coinsPlaced)
            {
                Transform coinPosition = levelIconContainers[currentLevel].transform.GetChild(coinsPlaced);
                GameObject coin = Instantiate(prefabCoin, coinPosition.position, Quaternion.identity);
                coin.GetComponent<Rigidbody2D>().isKinematic = true;
                instantiatedCoins.Add(coin);
            }

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
        wall.UpgradeWall();
        currentLevel++; // Increase the level after successful upgrade

        foreach (var coin in instantiatedCoins)
        {
            Destroy(coin);
        }
        instantiatedCoins.Clear();
        coinsPlaced = 0;

        // Update the icon visibility based on the new level, if player is still in range
        if (playerInRange) UpdateIconVisibility(true);
    }

    private void UpdateIconVisibility(bool isVisible)
    {
        // Ensure all containers are initially disabled
        foreach (var container in levelIconContainers)
        {
            container.SetActive(false);
        }

        // Then, based on visibility flag, enable the current level's container
        if (isVisible && currentLevel < levelIconContainers.Count)
        {
            levelIconContainers[currentLevel].SetActive(true);
        }
    }
}









