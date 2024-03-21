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
        // Check if the "R" key was pressed to request unit spawn
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Allow new spawn requests up to the maximum limit of the queue
            if (spawnRequests < maxQueue)
            {
                spawnRequests++; // Add a new request to the queue
                Debug.Log($"Spawn requested. {spawnRequests} units in queue.");

                // If a spawn isn't already scheduled, start the process
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

        // Remove null references if units are destroyed, to keep the list clean
        spawnedUnits.RemoveAll(item => item == null);
    }

    IEnumerator SpawnUnitWithDelay()
    {
        isSpawnScheduled = true; // Prevent multiple spawns from overlapping

        // As long as there are requests in the queue, continue spawning
        while (spawnRequests > 0)
        {
            yield return new WaitForSeconds(spawnDelay); // Wait for the spawn delay

            // Spawn a unit and adjust the queue
            GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
            spawnedUnits.Add(newUnit); // Add the new unit to the spawned list
            spawnRequests--; // Decrement the number of spawn requests as one has been fulfilled
            Debug.Log($"Unit spawned. Remaining requests in queue: {spawnRequests}. Total units spawned: {spawnedUnits.Count}");
        }

        isSpawnScheduled = false; // Allow new spawns to be scheduled
    }
}
