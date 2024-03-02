using UnityEngine;
using System;

public class RallyPointManager : MonoBehaviour
{
    public static RallyPointManager Instance { get; private set; }
    public Transform CurrentRallyPoint { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Wall.OnWallConstructed += UpdateRallyPoint;
    }

    private void OnDisable()
    {
        Wall.OnWallConstructed -= UpdateRallyPoint;
    }

    private void UpdateRallyPoint(Wall newWall)
    {
        CurrentRallyPoint = newWall.transform;
        Debug.Log("Updated Rally Point to " + newWall.name);
    }
    public void UpdateRallyPoint(Transform newRallyPoint)
    {
        CurrentRallyPoint = newRallyPoint;
        Debug.Log("Updated Rally Point to " + newRallyPoint.name);
    }


}
