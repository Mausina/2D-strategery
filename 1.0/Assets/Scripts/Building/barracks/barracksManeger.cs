using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barracksManager : MonoBehaviour
{
    public GameObject unitPrefab; // Reference to your unit prefab
    public float spawnDelay = 15f; // Delay between spawns, in seconds
    private int maxQueue = 4; // Maximum number of units that can be in the queue
    private List<GameObject> spawnedUnits = new List<GameObject>(); // List to track spawned units
    private int spawnRequests = 0; // Number of units the player has requested to spawn
    private bool isSpawnScheduled = false; // Flag to indicate if a spawn is scheduled to prevent overlap

    void Update()
    {
        // Remove null references if units are destroyed, to keep the list clean
        spawnedUnits.RemoveAll(item => item == null);
    }

    public bool CanAddToQueue()
    {
        // Check if new units can be added to the queue
        return spawnRequests < maxQueue;
    }

    public void AddToQueue()
    {
        // Add a new request to the queue and start the spawning process if not already started
        if (CanAddToQueue())
        {
            spawnRequests++;
            Debug.Log($"Spawn requested. {spawnRequests} units in queue.");
            if (!isSpawnScheduled)
            {
                StartCoroutine(SpawnUnitWithDelay());
            }
        }
        else
        {
            Debug.Log("Maximum queue capacity reached. Wait for units to spawn.");
        }
    }

    public int GetQueueSize()
    {
        // Return the total size of the queue, including units waiting to spawn and units already spawned
        return spawnRequests;
    }

    IEnumerator SpawnUnitWithDelay()
    {
        isSpawnScheduled = true;

        while (spawnRequests > 0)
        {
            yield return new WaitForSeconds(spawnDelay);

            GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
            spawnedUnits.Add(newUnit);
            spawnRequests--;
            Debug.Log($"Unit spawned. {spawnRequests} units remain in queue. Total units spawned: {spawnedUnits.Count}");
        }

        isSpawnScheduled = false;
    }
}

