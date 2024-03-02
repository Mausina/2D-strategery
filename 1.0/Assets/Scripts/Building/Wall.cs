using UnityEngine;
using System; // Necessary for EventHandler

public class Wall : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 3;
    public Coin[] coins;
    public WallLevel[] wallLevels;

    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    public static event Action<Wall> OnWallConstructed;

    private void Awake()
    {
        // Notify that a new wall has been constructed
        OnWallConstructed?.Invoke(this);
    }
    public int Level { get; private set; } = 1;
    // Use this event to notify when the wall level changes
    public event EventHandler LevelChanged;
    public bool IsMaxLevel
    {
        get { return level >= maxLevel; }
    }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateWallVisual();
        InitializeCoins();
       // RallyPointManager.Instance.UpdateRallyPoint(transform);
    }
    private void InitializeCoins()
    {
        // Initialize the state of each coin based on the level
        coins = new Coin[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            coins[i] = new Coin(); // Assuming you have a default state for a Coin
            coins[i].isFilled = i < level; // Fill the coins according to the current level
            // You would need to implement the visual representation of coins here
        }
    }
    public int GetCurrentUpgradeCost()
    {
        // Check if the current level is less than the max level and that the wallLevels array is properly initialized
        if (level < maxLevel && wallLevels != null && level - 1 < wallLevels.Length)
        {
            // Return the upgrade cost for the current level
            return wallLevels[level - 1].upgradeCost; // Adjust for zero-based indexing
        }
        else
        {
            // If the wall is at max level or there's an issue with the wallLevels array, return 0 as there's no upgrade cost
            return 0;
        }
    }

    public void UpgradeWall()
    {
        if (level < maxLevel)
        {
            WallLevel nextLevelInfo = wallLevels[level - 1]; // Correct indexing
            int upgradeCost = nextLevelInfo.upgradeCost;

            if (CoinManager.Instance.CanAfford(upgradeCost))
            {
                CoinManager.Instance.SpendCoins(upgradeCost);
                level++;
                UpdateWallVisual();
                LevelChanged?.Invoke(this, EventArgs.Empty);
                coins[level - 1].isFilled = true; // Assuming this is correct since coins array is likely 0-indexed

                // Check if the rally point should be updated
                if (level == 2) // After increment, which means it was level 1 before increment
                {
                    RallyPointManager.Instance.UpdateRallyPoint(this);
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



    private GameObject currentWallInstance; // Holds the current wall instance

    void UpdateWallVisual()
    {
        if (wallLevels != null && level - 1 < wallLevels.Length)
        {
            WallLevel currentLevel = wallLevels[level - 1];

            // Ensure the old wall instance is removed.
            if (currentWallInstance != null)
            {
                Destroy(currentWallInstance);
            }

            // Instantiate the new wall prefab, ensuring it's at the correct position and scale.
            if (currentLevel.wallPrefab != null)
            {
                currentWallInstance = Instantiate(currentLevel.wallPrefab, transform.position, transform.rotation);
                // Optionally, set the parent of the wall to organize your scene hierarchy.
                // currentWallInstance.transform.SetParent(transform, false);

                // Ensure the new prefab instance has the correct scale.
                // If your prefab needs to be scaled, adjust it here. Example:
                // currentWallInstance.transform.localScale = new Vector3(1, 1, 1);

                currentHealth = currentLevel.health;
            }
        }
    }



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Handle the wall being destroyed here
        }
    }
    public void PlayerStoppedFilling()
    {
        // Call this method when the player stops filling the coins
        // Make the unfilled coins fall
        foreach (var coin in coins)
        {
            if (!coin.isFilled)
            {
                // Implement the logic for a coin to fall
                // This may involve playing an animation, reducing its position, etc.
            }
        }
    }
}

[System.Serializable]
public class WallLevel
{
    public GameObject wallPrefab; // This is the prefab for the wall at each level
    public int health;
    public int upgradeCost;
}

[System.Serializable]
public class Coin
{
    public bool isFilled; // Represents if the coin is filled or not
    // Add any other properties you need, like a reference to the GameObject, position, etc.
}





