using UnityEngine;
using System;

public class Wall : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 3;
    public Coin[] coins;
    public WallLevel[] wallLevels;
    public BuilderController builderController; // Ensure this has the necessary logic for building/upgrading
    [SerializeField] public GameObject nightPoint; // Corrected naming convention
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    public static event Action<Wall> OnWallConstructed;
    public event Action OnWallUpgraded;
    private GameObject currentWallInstance; // Add this line near the top of your Wall class

    private void Awake()
    {
        OnWallConstructed?.Invoke(this);
        Level = 1; // Initialize level with property to trigger any setter logic
        InitializeCoins();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateWallVisual();
    }

    public int Level { get; private set; } = 1;

    public event EventHandler LevelChanged;

    public bool IsMaxLevel => level >= maxLevel;

    private void InitializeCoins()
    {
        coins = new Coin[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            coins[i] = new Coin { isFilled = i < level };
            // TODO: Implement visual representation for coins here.
        }
    }

    public int GetCurrentUpgradeCost()
    {
        if (level < maxLevel && wallLevels != null && level - 1 < wallLevels.Length)
        {
            return wallLevels[level - 1].upgradeCost;
        }
        return 0;
    }
    public bool CanPlaceCoin()
    {
        // Example logic: Check if the current level has room for more coins
        return coins.Length > level - 1 && !coins[level - 1].isFilled;
    }

    public Vector3 GetNextCoinPosition()
    {
        // Example logic: Return a position for the next coin
        // This is a placeholder. You need to implement according to your game's design
        return transform.position + new Vector3(0, 1, 0); // Adjust this based on your needs
    }

    public bool CheckIfUpgradeComplete(int coinsPlaced)
    {
        // Example logic: Check if the number of placed coins completes the upgrade for the current level
        // This assumes each level requires a fixed number of coins equal to 'coinsNeededPerLevel'
        int coinsNeededPerLevel = 1; // Placeholder, adjust as necessary
        return coinsPlaced >= coinsNeededPerLevel;
    }

    public void UpgradeWall()
    {
        if (level < maxLevel)
        {
            WallLevel nextLevelInfo = wallLevels[level - 1];
            int upgradeCost = nextLevelInfo.upgradeCost;

            // CoinManager check here. Make sure CoinManager is correctly implemented and accessible
            if (CoinManager.Instance.CanAfford(upgradeCost))
            {
                CoinManager.Instance.SpendCoins(upgradeCost);
                level++;
                UpdateWallVisual();
                LevelChanged?.Invoke(this, EventArgs.Empty);
                OnWallUpgraded?.Invoke();
                // Update coins logic here
                coins[level - 1].isFilled = true;
                // Update RallyPoint if necessary
                if (level == 2)
                {
                    // Ensure RallyPointManager is implemented
                    RallyPointManager.Instance.UpdateRallyPointToTransform(nightPoint.transform);
                }
            }
            else
            {
                Debug.Log("Not enough coins to upgrade!");
            }
        }
        else
        {
            Debug.Log("Wall is already at maximum level!");
        }
    }

    void UpdateWallVisual()
    {
        // Implement the logic to visually represent wall upgrades
        if (wallLevels != null && level - 1 < wallLevels.Length)
        {
            WallLevel currentLevel = wallLevels[level - 1];
            if (currentWallInstance != null)
            {
                Destroy(currentWallInstance);
            }

            if (currentLevel.wallPrefab != null)
            {
                Vector3 spawnPosition = GetGroundPosition(transform.position);
                currentWallInstance = Instantiate(currentLevel.wallPrefab, spawnPosition, Quaternion.identity);
                currentHealth = currentLevel.health;
            }
        }
    }

    Vector3 GetGroundPosition(Vector3 startPosition)
    {
        RaycastHit hit;
        float rayStartHeight = 10f;
        Vector3 rayStart = new Vector3(startPosition.x, startPosition.y + rayStartHeight, startPosition.z);
        float maxDistance = 20f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, maxDistance))
        {
            return hit.point;
        }
        return startPosition; // Fallback to the original position if no ground is found
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Implement wall destruction logic here
        }
    }

    public void PlayerStoppedFilling()
    {
        foreach (var coin in coins)
        {
            if (!coin.isFilled)
            {
                // Logic for making unfilled coins fall or handling player stop action
            }
        }
    }
}

[System.Serializable]
public class WallLevel
{
    public GameObject wallPrefab;
    public int health;
    public int upgradeCost;
    public int timeForUpgrade; // Corrected naming convention
}

[System.Serializable]
public class Coin
{
    public bool isFilled;
    // Implement additional properties as needed
}
