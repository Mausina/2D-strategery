using System;
using UnityEngine;

public class MobPortal : MonoBehaviour
{
    public GameObject mobPrefab; // Prefab of the mob to spawn
    public Transform spawnPoint;

    private int nightCount = 1; // Track the night count
    private int mobsToSpawn = 1; // Number of mobs to spawn initially
    private int lastSpawnedNight = 0; // Track the last night when mobs were spawned

    private void Start()
    {
        // Find the WorldTime component in the scene
        WorldTimeSystem.WorldTime worldTime = FindObjectOfType<WorldTimeSystem.WorldTime>();
        if (worldTime != null)
        {
            // Subscribe to the WorldTimeChanged event
            worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }
        else
        {
            Debug.LogError("WorldTime component not found in the scene.");
        }
    }

    private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
    {
        // Calculate the current night
        int currentNight = (int)Math.Floor(currentTime.TotalDays) + 1;

        // Check if it's a new night and mobs haven't been spawned for this night
        if (currentNight > lastSpawnedNight && (currentTime.Hours >= 20 || currentTime.Hours < 6))
        {
            lastSpawnedNight = currentNight; // Update the last spawned night
            SpawnMobs();
            IncreaseMobsToSpawn(); // Increase mobs to spawn for the next night
        }
    }

    private void SpawnMobs()
    {
        for (int i = 0; i < mobsToSpawn; i++)
        {
            // Instantiate a mob prefab at the spawn point
            GameObject newMob = Instantiate(mobPrefab, spawnPoint.position, Quaternion.identity);

            // Trigger animation on the mob prefab or spawn point GameObject
            // Example:
            // newMob.GetComponent<Animator>().SetTrigger("Spawn");
        }
    }

    private void IncreaseMobsToSpawn()
    {
        // Increase the number of mobs to spawn for the next night
        mobsToSpawn += 2; // Increase by 2 each night (adjust as needed)
        nightCount++;
        Debug.Log("Night " + nightCount + ": Mobs to spawn - " + mobsToSpawn);
    }
}






