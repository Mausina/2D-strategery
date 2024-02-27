using UnityEngine;

public class Wall : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 3;
    public WallLevel[] wallLevels; // Now this is an array of WallLevel instances

    private SpriteRenderer spriteRenderer;
    private int currentHealth;

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeWall();
        }
    }
    */

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateWallVisual(); // Update the visual on start
    }

    public void UpgradeWall()
    {
        if (level < maxLevel)
        {
            WallLevel nextLevelInfo = wallLevels[level]; // Get the next level's info
            int upgradeCost = nextLevelInfo.upgradeCost; // Get the upgrade cost for the next level

            // Assuming you have a method to get the current coins the player has
            if (CoinManager.Instance.CanAfford(upgradeCost)) // Check if the player can afford the upgrade
            {
                CoinManager.Instance.SpendCoins(upgradeCost); // Deduct the coins for the upgrade
                level++;
                UpdateWallVisual(); // Update the wall's visual and stats
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
        if (wallLevels != null && level - 1 < wallLevels.Length)
        {
            WallLevel currentLevel = wallLevels[level - 1];
            spriteRenderer.sprite = currentLevel.wallSprite;
            currentHealth = currentLevel.health; // Set health for the current level
            // Update other stats as needed
        }
    }

    // Call this method to damage the wall
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth.ToString());
        if (currentHealth <= 0)
        {

            // Handle the wall being destroyed
            // You might disable the wall, show a destroyed state, etc.
        }
    }
}
[System.Serializable] // This makes the class show up in the Unity inspector
public class WallLevel
{
    public Sprite wallSprite; // The sprite for this level
    public int health; // The health for this level
    public int upgradeCost; // The cost to upgrade to this level
    // Add any other properties you want for each level, such as damage resistance, etc.
}



