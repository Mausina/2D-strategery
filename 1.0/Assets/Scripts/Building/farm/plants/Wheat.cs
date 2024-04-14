using System;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using WorldTimeSystem;

namespace WorldTimeSystem
{

    public class Wheat : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> growthStagePrefabs; // Prefabs for each growth stage
        [SerializeField]
        private GameObject deterioratedPrefab; // Prefab for the deteriorated crop
        [SerializeField]
        private GameObject prefabCoin; // Prefab to drop when harvested
        [SerializeField]
        private int numberOfCoins = 5; // Number of coins to spawn
        [SerializeField]
        private float spawnRadius = 1.0f; // Radius within which coins will spawn

        private int currentStage = -1; // Start at -1 to indicate not growing
        private const int GROWTH_DAYS = 3; // Total days for full growth
        private const int NUM_STAGES = 7; // Number of growth stages
        private GameObject currentCropInstance; // The current instance of the crop
        private TimeSpan lastGrowthTime; // Last time the crop grew
        private bool isFarmerInteracted = false; // Flag to check if farmer has interacted
        private TimeSpan timeWhenFullyGrown; // Time when the crop reached full growth

        private void Start()
        {
            WorldTime.Instance.WorldTimeChanged += OnWorldTimeChanged;
            lastGrowthTime = WorldTime.Instance.GetCurrentTime();
        }

        private void OnDestroy()
        {
            WorldTime.Instance.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
        {
            if (!isFarmerInteracted) return;

            // Calculate the interval in real-time seconds for each growth stage
            float totalGrowthTimeInSeconds = GROWTH_DAYS * WorldTime.Instance._dayLength;
            float growthIntervalInSeconds = totalGrowthTimeInSeconds / NUM_STAGES;

            float secondsSinceLastGrowth = (float)(currentTime - lastGrowthTime).TotalMinutes;

            if (currentStage < NUM_STAGES - 1 && secondsSinceLastGrowth >= growthIntervalInSeconds)
            {
                lastGrowthTime = currentTime;
                GrowCrop();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Farmer") && !isFarmerInteracted)
            {
                isFarmerInteracted = true;
                Invoke("InitialGrow", 7f); // Start growing after a 7-second delay
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Farmer") && currentStage == NUM_STAGES - 1)
            {
                Harvest();
            }
        }

        private void InitialGrow()
        {
            lastGrowthTime = WorldTime.Instance.GetCurrentTime();
            GrowCrop();
        }

        private void GrowCrop()
        {
            if (currentStage < NUM_STAGES - 1)
            {
                currentStage++;
                if (currentCropInstance != null)
                {
                    Destroy(currentCropInstance);
                }
                currentCropInstance = Instantiate(growthStagePrefabs[currentStage], transform.position, Quaternion.identity, transform);

                if (currentStage == NUM_STAGES - 1)
                {
                    timeWhenFullyGrown = WorldTime.Instance.GetCurrentTime();
                }
            }
        }

        private void DeteriorateCrop()
        {
            currentStage = NUM_STAGES;
            if (currentCropInstance != null)
            {
                Destroy(currentCropInstance);
            }
            currentCropInstance = Instantiate(deterioratedPrefab, transform.position, Quaternion.identity, transform);
        }

        private void Harvest()
        {
            if (currentCropInstance != null)
            {
                Destroy(currentCropInstance);
            }

            // Calculate the top edge of the collider
            var collider = GetComponent<Collider2D>();
            float topEdge = collider.bounds.max.y;

            // Spawn coins along the top edge of the collider
            for (int i = 0; i < numberOfCoins; i++)
            {
                float spawnX = UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x);
                Vector3 spawnPosition = new Vector3(spawnX, topEdge, 0);
                Instantiate(prefabCoin, spawnPosition, Quaternion.identity);
            }

            Destroy(gameObject); // Consider not destroying if you want the crop to regrow
        }
    }
}
