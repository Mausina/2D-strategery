using System.Collections;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public float speed = 5f; // Movement speed of the builder.
    public float stamina = 10f; // Builder's stamina for continuous movement.
    public float staminaThreshold = 3f; // Minimum stamina required to start moving.
    private BuildingList buildingList; // Reference to the BuildingList component containing buildings to upgrade.
    private Animator animator; // Animator component for controlling the builder's animations.
    private bool isMoving = false; // Flag to track if the builder is currently moving.
    private bool isMovingRight = true; // Indicates the builder's current moving direction.
    private bool isUpgrading = false; // Indicates if the builder is currently upgrading a building.

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        buildingList = FindObjectOfType<BuildingList>();
        if (buildingList == null)
        {
            Debug.LogError("BuildingList not found in the scene.");
            return;
        }
        StartCoroutine(MoveAndUpgradeBuildings());
    }

    private IEnumerator MoveAndUpgradeBuildings()
    {
        while (true)
        {
            if (buildingList.buildingsToUpgrade.Count > 0 && !isMoving && !isUpgrading && stamina > staminaThreshold)
            {
                Transform targetBuilding = buildingList.buildingsToUpgrade[0];
                yield return StartCoroutine(MoveToPosition(targetBuilding.position));
                ActivateBuilding(targetBuilding);
                yield return new WaitUntil(() => !isMoving && !isUpgrading);
            }
            if (!isMoving)
            {
                stamina = Mathf.Min(stamina + Time.deltaTime * 2, 10f);
            }
            yield return null;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        animator.SetBool("isMoving", true);
        while (Vector3.Distance(transform.position, targetPosition) > 0.7f)
        {
            isMovingRight = targetPosition.x > transform.position.x;
            AdjustFacingDirection();
            Vector3 step = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = step;
            yield return null;
        }
        animator.SetBool("isMoving", false);
        isMoving = false;
    }

    private void ActivateBuilding(Transform building)
    {
       
        Debug.Log($"Activating Building: {building.name}");
        var upgradeBuildingAnimation = building.GetComponent<UpgradeBuildingAnimatio>();
       
        if (upgradeBuildingAnimation != null)
        {
            buildingList.UpdateBuilderCount(building, true); // Increment builder count
            float animationSpeedMultiplier = buildingList.CalculateAnimationSpeedMultiplier(building); // Calculate speed multiplier

            upgradeBuildingAnimation.SetAnimatorSpeed(animationSpeedMultiplier); // Adjust animator speed based on builder count

            animator.SetBool("isBuilding", true);
            isUpgrading = true;

            // Check if there's an ongoing upgrade coroutine and update its speed and remaining time accordingly.
            if (buildingList.upgradeCoroutines.ContainsKey(building))
            {
                // Update the remaining time with the time that has already passed, considering the new speed multiplier.
                float timePassed = buildingList.buildingUpgradeTimeList[buildingList.buildingsToUpgrade.IndexOf(building)] - buildingList.remainingUpgradeTimes[building];
                float newRemainingTime = buildingList.remainingUpgradeTimes[building] - (timePassed * (animationSpeedMultiplier - 1));
                buildingList.remainingUpgradeTimes[building] = Mathf.Max(newRemainingTime, 0); // Ensuring we don't go negative

                Debug.Log($"Building '{building.name}' already upgrading. Time passed: {timePassed}, New Remaining Time: {buildingList.remainingUpgradeTimes[building]}");
            }
            else
            {
                int buildingIndex = buildingList.buildingsToUpgrade.IndexOf(building);
                if (buildingIndex != -1)
                {
                    float upgradeTime = buildingList.buildingUpgradeTimeList[buildingIndex];
                    buildingList.remainingUpgradeTimes[building] = upgradeTime; // Set initial remaining time
                    Coroutine upgradeCoroutine = StartCoroutine(RepeatUpgradeAnimation(building));
                    buildingList.upgradeCoroutines[building] = upgradeCoroutine; // Store the coroutine reference
                }
                else
                {
                    Debug.LogError($"Index for {building.name} not found in upgrade time list.");
                }
            }
        }
        else
        {
            Debug.LogError($"UpgradeBuildingAnimatio component not found on {building.name}.");
        }
    }





    private IEnumerator RepeatUpgradeAnimation(Transform building)
    {
        var upgradeBuildingAnimation = building.GetComponent<UpgradeBuildingAnimatio>();

        if (upgradeBuildingAnimation == null)
        {
            Debug.LogError($"UpgradeBuildingAnimation component not found on {building.name}. Coroutine will not run.");
            yield break;
        }

        Debug.Log($"Starting RepeatUpgradeAnimation coroutine for {building.name}.");

        while (buildingList.remainingUpgradeTimes[building] > 0)
        {
            // Set the upgrade animation state to true, indicating that the upgrade is in progress.
            upgradeBuildingAnimation.SetUpgradeAnimationState(true);

            // Calculate the current animation speed multiplier based on the number of builders.
            float animationSpeedMultiplier = buildingList.CalculateAnimationSpeedMultiplier(building);
            // Log the current animation speed setting.
            Debug.Log($"[RepeatUpgradeAnimation] {building.name} - Animation speed set to {animationSpeedMultiplier}.");

            // Wait for an adjusted amount of time based on the current speed multiplier.
            // This effectively speeds up or slows down the upgrade process based on the number of builders.
            float timeToWait = 1 / animationSpeedMultiplier;
            float adjustedRemainingTime = buildingList.remainingUpgradeTimes[building] * animationSpeedMultiplier;

            // Check if the adjusted remaining time is less than 0.6 seconds.
            if (adjustedRemainingTime <= 0.6)
            {
                // If less than 0.6 seconds are left, skip the wait and set the remaining time to 0 to complete the upgrade.
                buildingList.remainingUpgradeTimes[building] = 0;
                Debug.Log($"[RepeatUpgradeAnimation] {building.name} - Less than 0.6 seconds remaining. Skipping to end of upgrade.");
            }
            else
            {
                // If more than 0.6 seconds are left, wait for the adjusted amount of time.
                yield return new WaitForSeconds(timeToWait);
                // Decrement the remaining upgrade time by the adjusted time.
                buildingList.remainingUpgradeTimes[building] -= timeToWait;
                Debug.Log($"[RepeatUpgradeAnimation] {building.name} - Decreased remaining time by {timeToWait}, new remaining time: {buildingList.remainingUpgradeTimes[building]}");
            }
            // Decrement the remaining upgrade time by the adjusted time, accounting for the speed multiplier.
            buildingList.remainingUpgradeTimes[building] -= timeToWait;
            // Log the updated remaining time after the adjustment.
            Debug.Log($"[RepeatUpgradeAnimation] {building.name} - Decreased remaining time by {timeToWait}, new remaining time: {buildingList.remainingUpgradeTimes[building]}");

            upgradeBuildingAnimation.SetUpgradeAnimationState(false);

            if (buildingList.remainingUpgradeTimes[building] <= 0)
            {
                Debug.Log($"Upgrade complete for {building.name}. Exiting coroutine.");
                break; // Exit loop if timeLeft has been exhausted.
            }
        }

        upgradeBuildingAnimation.CompleteUpgrade();
        buildingList.upgradeCoroutines.Remove(building); // Clean up the coroutine reference
        buildingList.remainingUpgradeTimes.Remove(building); // Clean up the remaining time
        buildingList.buildingUpgradePending[building] = false;
        OnUpgradeComplete(building); // Trigger upgrade completion event or logic
        Debug.Log($"[RepeatUpgradeAnimation] {building.name} - Coroutine finished. Upgrade and cleanup done.");
    }






    public void OnUpgradeComplete(Transform building)
    {
        animator.SetBool("isBuilding", false);
        isUpgrading = false;
        int buildingIndex = buildingList.buildingsToUpgrade.IndexOf(building);
        if (buildingIndex != -1)
        {
            buildingList.buildingsToUpgrade.Remove(building);
            if (buildingList.buildingUpgradeTimeList.Count > buildingIndex)
            {
                buildingList.buildingUpgradeTimeList.RemoveAt(buildingIndex);
            }
            buildingList.UpdateBuilderCount(building, false); // Decrement the builder count.
        }
    }



    private void AdjustFacingDirection()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}