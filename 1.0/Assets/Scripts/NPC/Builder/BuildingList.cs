using UnityEngine;
using System.Collections.Generic;

public class BuildingList : MonoBehaviour
{
    public BuilderPoolManager builderPoolManager;
    public List<float> buildingUpgradeTimeList = new List<float>();
    [SerializeField]
    private List<KeyValuePair<GameObject, int>> buildingsToUpgrade = new List<KeyValuePair<GameObject, int>>();

    private void Awake()
    {
        builderPoolManager = FindObjectOfType<BuilderPoolManager>();
    }

    public void AddBuildingToUpgradeList(GameObject building, int howManyNeedBuilders, float timeForUpgrade)
    {
        buildingsToUpgrade.Add(new KeyValuePair<GameObject, int>(building, howManyNeedBuilders));
        buildingUpgradeTimeList.Add(timeForUpgrade);
        Debug.Log($"Added building '{building.name}' for upgrade with {howManyNeedBuilders} builders needed and upgrade time {timeForUpgrade} seconds.");
        SortBuildingsByPriority(); // Sort buildings after adding a new one
        AssignBuildersToBuildings();
    }

    private void SortBuildingsByPriority()
    {
        buildingsToUpgrade.Sort((pair1, pair2) =>
        {
            if (pair1.Key.tag == "Wall" && pair2.Key.tag != "Wall") return -1;
            if (pair2.Key.tag == "Wall" && pair1.Key.tag != "Wall") return 1;
            if (pair1.Key.tag == "Tower" && pair2.Key.tag != "Tower") return -1;
            if (pair2.Key.tag == "Tower" && pair1.Key.tag != "Tower") return 1;
            return 0;
        });
        Debug.Log("Sorted buildings by priority. (Walls > Towers > Others)");
    }

    private void AssignBuildersToBuildings()
    {
        Debug.Log($"Starting assignment of builders to buildings. Total Buildings: {buildingsToUpgrade.Count}");
        foreach (var buildingPair in buildingsToUpgrade)
        {
            var building = buildingPair.Key;
            var neededBuilders = buildingPair.Value;
            var assignedBuilders = 0;

            Debug.Log($"Attempting to assign builders to {building.name}, Needed Builders: {neededBuilders}");

            while (assignedBuilders < neededBuilders)
            {
                var builder = builderPoolManager.RequestBuilder();
                if (builder != null)
                {
                    Debug.Log($"Assigned builder to {building.name}. Total Assigned: {assignedBuilders + 1}/{neededBuilders}");
                    builder.AssignToBuild(building);
                    assignedBuilders++;
                }
                else
                {
                    Debug.LogWarning($"No available builder to assign to {building.name}. Needed: {neededBuilders}, Assigned: {assignedBuilders}");
                    break; // Break if no builder is available, wait for next opportunity
                }
            }
        }
    }

    public void NotifyNewBuilderAvailable()
    {
        Debug.Log("New builder available. Reassigning builders to buildings.");
        AssignBuildersToBuildings();
    }
}
