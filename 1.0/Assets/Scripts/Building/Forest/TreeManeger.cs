using System;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public int level = 1;
    public int maxLevel = 3;
    private BuildingList buildingList;

    public static event Action<ObjectUpgrade> OnWallConstructed;

    public int Level { get; private set; } = 1;

    public event EventHandler LevelChanged;

    public bool IsMaxLevel => level >= maxLevel;


    public void TreeCutDown()
    {
        Debug.Log("I give a signal to the builder\r\n");
        Destroy(gameObject);
    }

}
[System.Serializable]
public class TreeAdjust
{
    public int timeForUpgrade; 
}

 