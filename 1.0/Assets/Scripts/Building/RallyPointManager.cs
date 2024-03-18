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
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        // When the singleton instance is destroyed, clear the static reference
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Updates the rally point to the specified transform
    public void UpdateRallyPoint(Transform nightPoint)
    {
        CurrentRallyPoint = nightPoint;
        Debug.Log("Updated Rally Point to " + nightPoint.gameObject.name);
    }
}
