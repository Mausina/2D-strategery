using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManeger : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 2;
    private UpgradeBuildingAnimation buildingAnimatio;
    public void UpgradeTree()
    {
        if (level < maxLevel)
        {

        }
    }



}

[System.Serializable]
public class TreeLevel
{
    public int timeForUpgrade; // Corrected naming convention
}
