using System;
using UnityEngine;


public class ObjectUpgrade : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 3;
    public Coin[] coins;
    public WallLevel[] wallLevels;
    private BuilderController builderController; // Ensure this has the necessary logic for building/upgrading
    private BuildingList buildingList;
    private UpgradeBuildingAnimation buildingAnimatio;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    public static event Action<ObjectUpgrade> OnWallConstructed;
    public event Action OnWallUpgraded;
    private GameObject currentWallInstance; // Add this line near the top of your Wall class
    private bool Preparation = true;

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

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            WallLevel nextLevelInfo = wallLevels[level - 1];
            int upgradeCost = nextLevelInfo.upgradeCost;

            if (CoinManager.Instance.CanAfford(upgradeCost))
            {
                CoinManager.Instance.SpendCoins(upgradeCost);
                level++;
                
                // Efficiently get BuildingList component if not already referenced
                if (buildingList == null)
                {
                    buildingList = FindObjectOfType<BuildingList>();
                    if (buildingList == null)
                    {
                        Debug.LogError("BuildingList component not found in the scene.");
                        return; // Early return if BuildingList is not found to avoid further execution
                    }
                }

                
                // Unsubscribe to prevent calling it multiple times

                // Upgrade complete, update visuals and logic
                UpdateWallVisual();
                GameObject newWallInstance = UpdateWallVisual();
                if (newWallInstance != null)
                {
                    // Now that we have a new building instance, add it to the list
                    buildingList.AddBuildingToUpgradeList(newWallInstance.gameObject, nextLevelInfo.timeForUpgrade, nextLevelInfo.HowManyNeedBuilder);
                    if (buildingAnimatio != null)
                    {
                        Debug.Log("buildingAnimatio: "+ nextLevelInfo.timeForUpgrade);
                        // Start the upgrade animation with the specified duration
                       // buildingAnimatio.StartUpgradeAnimation(true, nextLevelInfo.timeForUpgrade);
                    }
                }
                LevelChanged?.Invoke(this, EventArgs.Empty);
                OnWallUpgraded?.Invoke();
                coins[level - 1].isFilled = true;

                if (level == 2)
                {
                    SafeZone safeZoneComponent = FindObjectOfType<SafeZone>();
                    if (safeZoneComponent != null)
                    {                     
                            // Assume the wall prefab has a sprite renderer and its size is used to determine the width
                            SpriteRenderer wallSpriteRenderer = newWallInstance.GetComponent<SpriteRenderer>();
                            if (wallSpriteRenderer != null)
                            {
                                float wallWidth = wallSpriteRenderer.bounds.size.x;
                                safeZoneComponent.ExpandSafeZone(newWallInstance.transform.position, wallWidth);
                            }
                    }
                    else
                    {
                        Debug.Log("SafeZone component not found.");
                    }
                }
                else
                {
                    Debug.Log("nightPoint is null");
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





    GameObject UpdateWallVisual()
    {
        // Destroy the old instance if it exists
        if (level > 1 && currentWallInstance != null)
        {
            Destroy(currentWallInstance);
        }

        // Create a new instance of the wall
        if (wallLevels != null && level - 1 < wallLevels.Length)
        {
            WallLevel currentLevel = wallLevels[level - 1];
            if (currentLevel.wallPrefab != null)
            {
                Vector3 spawnPosition = GetGroundPosition(transform.position);
                // Instantiate the new wall and assign it to currentWallInstance
                currentWallInstance = Instantiate(currentLevel.wallPrefab, spawnPosition, Quaternion.identity);
                currentHealth = currentLevel.health;
            }
        }
        return currentWallInstance; // Return the new instance
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


}

[System.Serializable]
public class WallLevel
{
    public GameObject wallPrefab;
    public int health;
    public int HowManyNeedBuilder;
    public int upgradeCost;
    public int timeForUpgrade; // Corrected naming convention
}

[System.Serializable]
public class Coin
{
    public bool isFilled;
    // Implement additional properties as needed
}


