using System;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public float spawnRate = 2f;
    private GameObject player;
    public float moveSpeed = 5f;
    public bool NigthAttack;
    private float nextSpawnTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && player != null)
        {
            if (NigthAttack)
            {
                if (IsNightTime())
                {
                    SpawnObject();
                    nextSpawnTime = Time.time + 1f / spawnRate;
                }
            }
            else
            {
                SpawnObject();
                nextSpawnTime = Time.time + 1f / spawnRate;
            }


        }
    }
    private bool IsNightTime()
    {
        TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
        UnityEngine.Debug.Log(currentTime);
        return currentTime.Hours < 6 || currentTime.Hours >= 18;
    }
    void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
        MovingObject movingScript = spawnedObject.AddComponent<MovingObject>(); // Add the movement script to the spawned object
        movingScript.target = player.transform; // Set the target to the player
        movingScript.moveSpeed = moveSpeed;
    }
}

// Separate script for the moving object
public class MovingObject : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5f;

    void Update()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            transform.position += directionToTarget * moveSpeed * Time.deltaTime;
        }
    }
}
