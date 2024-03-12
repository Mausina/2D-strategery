using System.Collections.Generic;
using UnityEngine;

public class BuildingList : MonoBehaviour
{
    public List<Transform> buildingsToUpgrade = new List<Transform>();
    public List<float> buildingUpgradeTimeList = new List<float>();
    public Dictionary<Transform, int> buildersPerBuilding = new Dictionary<Transform, int>();

    public void AddBuildingToUpgradeList(Transform building, float timeForUpgrade)
    {
        if (!buildingsToUpgrade.Contains(building))
        {
            buildingsToUpgrade.Add(building);
            buildingUpgradeTimeList.Add(timeForUpgrade);
            buildersPerBuilding[building] = 0; // Initialize builder count for this building.
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
    }

    public int GetBuilderCount(Transform building)
    {
        if (buildersPerBuilding.ContainsKey(building))
        {
            return buildersPerBuilding[building];
        }
        return 0;
    }

    // Utility method to calculate animation speed multiplier
    public float CalculateAnimationSpeedMultiplier(Transform building)
    {
        int numberOfBuilders = GetBuilderCount(building);
        float multiplier = 1f; // Default speed multiplier

        switch (numberOfBuilders)
        {
            case 1: multiplier = 1f; break; // Normal speed
            case 2: multiplier = 1.25f; break; // 25% faster
            case 3: multiplier = 1.5f; break; // 50% faster
            case 4: multiplier = 1.75f; break; // 75% faster
            default: multiplier = 1f; break; // Default to normal speed if out of expected range
        }

        Debug.Log($"Calculated Animation Speed Multiplier: x{multiplier} for {numberOfBuilders} builders.");
        return multiplier;
    }
}