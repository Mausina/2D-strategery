using System;
using System.Collections.Generic;
using UnityEngine;
using WorldTimeSystem;

namespace WorldTimeSystem
{
    public static class WorldTimeConstants
    {
        public const int MinutesInDay = 1440; // Example value for 24 hours
    }
    public class Wheat : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> growthStagePrefabs; // Prefabs for each growth stage
        [SerializeField]
        private GameObject deterioratedPrefab; // Prefab for the deteriorated crop
        [SerializeField]
        private GameObject prefabCoin; // Prefab to drop when harvested

        private int currentStage = -1; // Start at -1 to indicate not growing
        private const int GROWTH_DAYS = 3; // Total days for full growth
        private const int NUM_STAGES = 7; // Number of growth stages
        private GameObject currentCropInstance; // The current instance of the crop
        private TimeSpan lastGrowthTime; // Last time the crop grew
        private bool isFarmerInteracted = false; // Flag to check if farmer has interacted
        private TimeSpan timeWhenFullyGrown; // Time when the crop reached full growth

        private void Start()
        {
            // Subscribe to the WorldTimeChanged event
            WorldTime.Instance.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            // Unsubscribe from the WorldTimeChanged event
            WorldTime.Instance.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
        {
            if (!isFarmerInteracted) return; // Only proceed if farmer has interacted

            // Calculate the interval in minutes for each stage of growth
            float growthInterval = (GROWTH_DAYS * WorldTimeConstants.MinutesInDay) / NUM_STAGES;

            // Grow to the next stage if the interval has passed
            if (currentStage < NUM_STAGES - 1 && currentTime.TotalMinutes >= lastGrowthTime.TotalMinutes + growthInterval)
            {
                lastGrowthTime = currentTime;
                GrowCrop();
            }

            // Check if it's time to deteriorate after 2 days of being fully grown
            if (currentStage == NUM_STAGES - 1 && currentTime.TotalMinutes >= timeWhenFullyGrown.TotalMinutes + (2 * WorldTimeConstants.MinutesInDay))
            {
                DeteriorateCrop();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Farmer"))
            {
                if (currentStage == -1)
                {
                    lastGrowthTime = WorldTime.Instance.GetCurrentTime();
                    isFarmerInteracted = true;
                    GrowCrop();
                }
                else if (currentStage == NUM_STAGES - 1)
                {
                    Harvest();
                }
            }
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

                // If the crop has reached full growth, record the time
                if (currentStage == NUM_STAGES - 1)
                {
                    timeWhenFullyGrown = WorldTime.Instance.GetCurrentTime();
                }
            }
        }

        private void DeteriorateCrop()
        {
            if (currentStage != NUM_STAGES)
            {
                currentStage = NUM_STAGES;
                if (currentCropInstance != null)
                {
                    Destroy(currentCropInstance);
                }
                currentCropInstance = Instantiate(deterioratedPrefab, transform.position, Quaternion.identity, transform);
            }
        }

        private void Harvest()
        {
            if (currentCropInstance != null)
            {
                Destroy(currentCropInstance);
            }
            Instantiate(prefabCoin, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
