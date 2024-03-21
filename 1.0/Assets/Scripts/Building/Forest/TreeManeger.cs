using System;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevel = 2; // Assuming there's a maximum level
    [SerializeField] private int timeForUpgrade;

    private BuildingList buildingList; // Manages a list of buildings
    private UpgradeBuildingAnimation buildingAnimation; // Manages building animations

    public event Action OnWallUpgraded; // Consider renaming to OnUpgraded for generality
    private GameObject currentWallInstance; // Represents the current instance to upgrade

    public int Level
    {
        get => level;
        private set => level = value;
    }

    public event EventHandler LevelChanged;

    public bool IsMaxLevel => Level >= maxLevel;

    private void Awake()
    {
        Level = 1; // Initialize level, consider removing if redundant
    }

    public void UpgradeWall()
    {
        if (!IsMaxLevel)
        {
           
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

            // Assuming currentWallInstance is the instance you want to upgrade
            if (currentWallInstance != null)
            {
                // Add the current building instance to the list for upgrade
                buildingList.AddBuildingToUpgradeList(currentWallInstance.transform, timeForUpgrade);

                // Trigger any specific upgrade animations
                if (buildingAnimation != null)
                {
                    Debug.Log("Upgrade animation should be played here.");
                    // You might want to play a specific animation via buildingAnimation.Play();
                }

                // Increment level and notify subscribers
                Level++;
                LevelChanged?.Invoke(this, EventArgs.Empty);
                OnWallUpgraded?.Invoke(); // Consider renaming this event
            }
        }
        else
        {
            Debug.Log("The object is already at maximum level!");
        }
    }
}
