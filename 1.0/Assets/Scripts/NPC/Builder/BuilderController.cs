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
            int buildingIndex = buildingList.buildingsToUpgrade.IndexOf(building);
            if (buildingIndex != -1)
            {
                float upgradeTime = buildingList.buildingUpgradeTimeList[buildingIndex];
                Debug.Log($"Activating upgrade animation with duration: {upgradeTime}, speed multiplier: {animationSpeedMultiplier}");
                StartCoroutine(RepeatUpgradeAnimation(upgradeTime, upgradeBuildingAnimation, building));
            }
            else
            {
                Debug.LogError($"Index for {building.name} not found in upgrade time list.");
            }
        }
        else
        {
            Debug.LogError($"UpgradeBuildingAnimatio component not found on {building.name}.");
        }
    }



    private IEnumerator RepeatUpgradeAnimation(float duration, UpgradeBuildingAnimatio upgradeAnim, Transform building)
    {
        float timeLeft = duration;
        while (timeLeft > 0)
        {
            upgradeAnim.SetUpgradeAnimationState(true);
            yield return new WaitForSeconds(1);
            upgradeAnim.SetUpgradeAnimationState(false);
            timeLeft -= 1;
        }
        upgradeAnim.CompleteUpgrade();
        OnUpgradeComplete(building);
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