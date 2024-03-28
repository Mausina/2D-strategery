using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushManager : MonoBehaviour
{
    // Prefab of the object you want to spawn.
    public GameObject objectToSpawnPrefab;

    // The currently spawned object.
    private GameObject spawnedObject;

    // Lists to keep track of trees and animals within the trigger zone.
    public List<GameObject> detectedTrees = new List<GameObject>();
    public List<GameObject> detectedAnimals = new List<GameObject>();

    // Time in seconds to wait before spawning the object.
    public float timeForSpawn;

    // Coroutine for spawning the object.
    private Coroutine spawnCoroutine;

    private void UpdateSpawning()
    {
        // If there are no trees, and we haven't spawned the object yet, start the spawning coroutine.
        if (detectedTrees.Count == 0 && spawnedObject == null && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnAfterDelay(timeForSpawn));
        }
        // If there are trees, and we have spawned the object, destroy it and stop the coroutine.
        else if (detectedTrees.Count > 0 && spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null; // Make sure to set the reference to null after destroying.
            // Also stop the spawn coroutine if it's running.
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    // Coroutine to spawn the object after a delay.
    private IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Only spawn the object if there are still no trees detected.
        if (detectedTrees.Count == 0)
        {
            spawnedObject = Instantiate(objectToSpawnPrefab, transform.position, Quaternion.identity);
        }
        // Set the coroutine reference to null after the object is spawned.
        spawnCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if a tree enters the trigger zone.
        if (collision.CompareTag("Tree") || collision.CompareTag("SafeZone"))
        {
            detectedTrees.Add(collision.gameObject);
            UpdateSpawning();
        }
        // Optionally detect if an animal enters the trigger zone.
        else if (collision.CompareTag("Animal"))
        {
            detectedAnimals.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detect if a tree exits the trigger zone.
        if (collision.CompareTag("Tree") || collision.CompareTag("SafeZone"))
        {
            detectedTrees.Remove(collision.gameObject);
            UpdateSpawning();
        }
        // Optionally detect if an animal exits the trigger zone.
        else if (collision.CompareTag("Animal"))
        {
            detectedAnimals.Remove(collision.gameObject);
        }
    }

    // Optional method to check if animals can spawn.
    public bool CanSpawnAnimals()
    {
        return detectedTrees.Count == 0;
    }
}
