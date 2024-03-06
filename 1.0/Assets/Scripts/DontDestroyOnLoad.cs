using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public List<GameObject> wallPrefabs; // A list to hold multiple wall prefabs

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Make the script's GameObject persist across scenes

        foreach (var prefab in wallPrefabs)
        {
            CreateAndPreserveWallInstance(prefab);
        }
    }

    void CreateAndPreserveWallInstance(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject wallInstance = Instantiate(prefab); // Instantiate each prefab
            DontDestroyOnLoad(wallInstance); // Make each instance persist across scenes
        }
        else
        {
            Debug.LogWarning("Prefab is null in DontDestroyOnLoad");
        }
    }
}
