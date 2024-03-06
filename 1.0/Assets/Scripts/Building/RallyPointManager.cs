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
        // Update this line to use the correct method name
        //Wall.OnWallConstructed += UpdateRallyPointToTransform;
    }

    private void OnDisable()
    {
        // Update this line to use the correct method name
       // Wall.OnWallConstructed -= UpdateRallyPointToTransform;
    }

    public void UpdateRallyPointToTransform(Transform newRallyPointTransform)
    {
        CurrentRallyPoint = newRallyPointTransform;
        Debug.Log("Updated Rally Point to " + newRallyPointTransform.gameObject.name);
    }

}
