using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuildingList : MonoBehaviour
{
    public List<Transform> buildingsToUpgrade = new List<Transform>();
    public List<float> buildingUpgradeTimeList = new List<float>();
    public Dictionary<Transform, int> buildersPerBuilding = new Dictionary<Transform, int>();
    public Dictionary<Transform, Coroutine> upgradeCoroutines = new Dictionary<Transform, Coroutine>();
    public Dictionary<Transform, float> remainingUpgradeTimes = new Dictionary<Transform, float>();
    public Dictionary<Transform, bool> buildingUpgradePending = new Dictionary<Transform, bool>();

    public void AddBuildingToUpgradeList(Transform building, float timeForUpgrade)
    {
        if (!buildingsToUpgrade.Contains(building))
        {
            buildingUpgradePending[building] = true;
            buildingsToUpgrade.Add(building);
            buildingUpgradeTimeList.Add(timeForUpgrade);
            buildersPerBuilding[building] = 0; // Initialize builder count for this building.
            remainingUpgradeTimes[building] = timeForUpgrade; // Set the initial remaining time for upgrade.
            Debug.Log($"Added building {building.name} with initial upgrade time: {timeForUpgrade}");
        }
    }

    public void UpdateBuilderCount(Transform building, bool adding)
    {
        if (!buildersPerBuilding.ContainsKey(building))
        {
            buildersPerBuilding[building] = 0; // Initialize if not already present
        }

        if (adding)
        {
            buildersPerBuilding[building] = Mathf.Min(buildersPerBuilding[building] + 1, 4); // Max 4 builders
        }
        else
        {
            buildersPerBuilding[building] = Mathf.Max(buildersPerBuilding[building] - 1, 0); // Prevent negative count
        }
        Debug.Log($"{(adding ? "Added" : "Removed")} builder for '{building.name}'. Total builders now: {buildersPerBuilding[building]}");
    }

    public int GetBuilderCount(Transform building)
    {
        if (buildersPerBuilding.ContainsKey(building))
        {
            return buildersPerBuilding[building];
        }
        return 0;
    }

    public float CalculateAnimationSpeedMultiplier(Transform building)
    {
        int numberOfBuilders = GetBuilderCount(building);
        float multiplier = 1f; // Default speed multiplier

        switch (numberOfBuilders)
        {
            case 1: multiplier = 1f; break;
            case 2: multiplier = 1.25f; break;
            case 3: multiplier = 1.5f; break;
            case 4: multiplier = 1.75f; break;
            default: multiplier = 1f; break;
        }

        Debug.Log($"Calculated Animation Speed Multiplier: x{multiplier} for {numberOfBuilders} builders.");
        return multiplier;
    }
}
