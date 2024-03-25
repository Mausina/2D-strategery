using UnityEngine;
using System.Collections.Generic;

public class BuilderPoolManager : MonoBehaviour
{
    public GameObject builderPrefab;
    private Queue<BuilderController> availableBuilders = new Queue<BuilderController>();

    void Start()
    {
    }

    public BuilderController RequestBuilder()
    {
        if (availableBuilders.Count > 0)
        {
            var builder = availableBuilders.Dequeue();
            builder.gameObject.SetActive(true);
            return builder;
        }
        return null; // No automatic instantiation of new builders.
    }

    public void ReleaseBuilder(BuilderController builder)
    {
        builder.gameObject.SetActive(false);
        availableBuilders.Enqueue(builder);
    }

    private void AddBuilderToPool(BuilderController builder)
    {
        if (builder != null)
        {
            builder.gameObject.SetActive(false);
            availableBuilders.Enqueue(builder);
        }
    }
}

