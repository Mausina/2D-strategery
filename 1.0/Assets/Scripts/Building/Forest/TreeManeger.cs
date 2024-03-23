using System;
using UnityEngine;

public class TreeManager : MonoBehaviour
{

    public int level = 1;
    public int maxLevel = 2;
    private BuildingList buildingList;
    private UpgradeBuildingAnimation buildingAnimatio;
    public static event Action<ObjectUpgrade> OnWallConstructed;
    public event Action OnWallUpgraded;
    private bool Preparation = true;


    public int Level { get; private set; } = 1;

    public event EventHandler LevelChanged;

    public bool IsMaxLevel => level >= maxLevel;


    public void TreeCutDown()
    {
        Debug.Log("I give a signal to the builder\r\n");
    }

}
[System.Serializable]
public class TreeAdjust
{
    public int timeForUpgrade; 
}

