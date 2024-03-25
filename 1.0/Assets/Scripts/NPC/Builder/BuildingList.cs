using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildingList : MonoBehaviour
{
    public BuilderPoolManager builderPoolManager;
    private List<GameObject> buildingsToUpgrade = new List<GameObject>();
    private List<GameObject> buildingTimeForUpgrade = new List<GameObject>();
    private Dictionary<GameObject, int> buildingNeeds = new Dictionary<GameObject, int>();

    void Update()
    {
        AssignBuildersToBuildings();
    }

    public void AddBuildingToUpgradeList(GameObject building, int neededBuilders, int buildingTimeForUpgrade)
    {
        if (!buildingNeeds.ContainsKey(building))
        {
            buildingNeeds.Add(building, neededBuilders);
            SortBuildingsByPriority();
        }
    }

    private void SortBuildingsByPriority()
    {
        // Assuming priority is determined by the GameObject's tag for simplicity.
        buildingsToUpgrade = buildingNeeds.Keys.OrderBy(
            building => building.tag == "Wall" ? 0 : building.tag == "Tower" ? 1 : 2).ToList();
    }

    private void AssignBuildersToBuildings()
    {
        foreach (var building in buildingsToUpgrade)
        {
            int needs = buildingNeeds[building];
            while (needs > 0)
            {
                var builder = builderPoolManager.RequestBuilder();
                if (builder != null)
                {
                    builder.AssignToBuild(building);
                    needs--;
                }
                else
                {
                    break; // No available builders, try again later.
                }
            }
            buildingNeeds[building] = needs; // Update remaining needs.
        }
    }
}
