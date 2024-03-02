using UnityEngine;

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

    // Adjust the method to check the wall's level before updating the rally point
    public void UpdateRallyPoint(Wall newWall)
    {
        CurrentRallyPoint = newWall.transform;
        Debug.Log("Updated Rally Point to " + newWall.gameObject.name);
    }


}
