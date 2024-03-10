using System.Collections.Generic;
using UnityEngine;

public class BuildingList : MonoBehaviour
{
    public List<Transform> buildingsToUpgrade = new List<Transform>(); // List of buildings to upgrade
    public List<float> buildingUpgradeTimeList = new List<float>();

    public void AddBuildingToUpgradeList(Transform building, int timeForUpgrade)
    {
            buildingsToUpgrade.Add(building);

            buildingUpgradeTimeList.Add(timeForUpgrade);
    }
}
