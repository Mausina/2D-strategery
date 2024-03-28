using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;
    public float spawnInterval = 10f;

    private class BushData
    {
        public Transform bushTransform;
        public List<GameObject> animalPool;
        public int maxAnimals;

        public BushData(Transform transform)
        {
            bushTransform = transform;
            animalPool = new List<GameObject>();
            maxAnimals = Random.Range(3, 5); // Randomly decide between 3 and 4 slots
        }
    }

    private List<BushData> bushes = new List<BushData>();
    private int currentBushIndex = 0; // Add a tracker for the current bush being processed

    private void Awake()
    {
        StartCoroutine(AnimalSpawnRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bush"))
        {
            bushes.Add(new BushData(collision.transform));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bush"))
        {
            var bushToRemove = bushes.Find(bush => bush.bushTransform == collision.transform);
            if (bushToRemove != null)
            {
                foreach (var animal in bushToRemove.animalPool)
                {
                    Destroy(animal); // Consider using a more efficient pooling system
                }
                bushes.Remove(bushToRemove);
            }
        }
    }

    private IEnumerator AnimalSpawnRoutine()
    {
        while (true)
        {
            if (bushes.Count > 0)
            {
                var bush = bushes[currentBushIndex % bushes.Count]; // Loop through bushes
                RefillBushPool(bush);

                currentBushIndex++; // Move to the next bush for the next spawn cycle
            }

            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval before spawning the next animal
        }
    }

    private void RefillBushPool(BushData bush)
    {
        // Clean up any deactivated animals not caught by the event
        bush.animalPool.RemoveAll(animal => !animal.activeInHierarchy);

        // Spawn one animal if the bush is not at maximum capacity
        if (bush.animalPool.Count < bush.maxAnimals)
        {
            var spawnedAnimal = Instantiate(animalPrefab, bush.bushTransform.position, Quaternion.identity);
            var animalComponent = spawnedAnimal.GetComponent<AnimalController>(); // Ensure correct component
            if (animalComponent != null)
            {
                animalComponent.OnAnimalDeactivated += (animal) =>
                {
                    bush.animalPool.Remove(animal);
                };
            }
            bush.animalPool.Add(spawnedAnimal);
        }
    }
}
