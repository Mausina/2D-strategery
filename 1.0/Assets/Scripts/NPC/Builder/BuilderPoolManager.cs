using UnityEngine;
using System.Collections.Generic;

public class BuilderPoolManager : MonoBehaviour
{
    public GameObject builderPrefab;
    private Queue<BuilderController> availableBuilders = new Queue<BuilderController>();

    [SerializeField]
    private BuildingList buildingList;

    private void Awake()
    {
        buildingList = FindObjectOfType<BuildingList>();
    }

    public void ShowAvailableBuildersCount()
    {
        Debug.Log($"Available Builders in Pool: {availableBuilders.Count}");
    }

    public BuilderController RequestBuilder()
    {
        if (availableBuilders.Count > 0)
        {
            var builder = availableBuilders.Dequeue();
            Debug.Log($"Recycled a builder from the pool. Available builders now: {availableBuilders.Count}");
            builder.gameObject.SetActive(true);
            return builder;
        }
        else
        {
            Debug.Log("No builders in the pool; instantiating a new one.");
            var builderObj = Instantiate(builderPrefab);
            var builder = builderObj.GetComponent<BuilderController>();
            if (builder != null)
            {
                return builder;
            }
            else
            {
                Debug.LogError("Failed to instantiate a new builder properly.");
            }
        }
        return null; // In case instantiation fails or no BuilderController component is attached
    }

    public void ReleaseBuilder(BuilderController builder)
    {
        Debug.Log($"Releasing builder {builder.name} back to the pool.");
        builder.ResetBuilder(); // Reset any specific state of the builder
        builder.gameObject.SetActive(false);
        availableBuilders.Enqueue(builder);
        Debug.Log($"Builder {builder.name} is now inactive and back in the pool. Total available builders: {availableBuilders.Count}");
        buildingList.NotifyNewBuilderAvailable();

    }
}
