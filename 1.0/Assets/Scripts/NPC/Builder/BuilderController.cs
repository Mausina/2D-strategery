using System.Collections;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public float speed = 5f; // The speed at which the builder moves.
    private BuildingList buildingList; // Reference to BuildingList component.
    private Animator animator; // Animator to control the builder's animations.
    private bool isMoving = false; // Track if the builder is currently moving.
    private bool isMovingRight = true; // Default direction.
    private bool isUpgrading = false; // Flag to indicate if currently upgrading.

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
        }
        else
        {
            StartCoroutine(MoveAndUpgradeBuildings());
        }
    }

    private IEnumerator MoveAndUpgradeBuildings()
    {
        while (true)
        {
            if (buildingList.buildingsToUpgrade.Count > 0 && !isMoving && !isUpgrading)
            {
                isMoving = true;
                animator.SetBool("isMoving", true);
                Transform targetBuilding = buildingList.buildingsToUpgrade[0];

                yield return StartCoroutine(MoveToBuilding(targetBuilding));

                ActivateBuilding(targetBuilding);
                yield return new WaitUntil(() => !isMoving && !isUpgrading);
            }
            yield return null;
        }
    }

    private IEnumerator MoveToBuilding(Transform targetBuilding)
    {
        while (Vector3.Distance(transform.position, targetBuilding.position) > 0.1f)
        {
            isMovingRight = (targetBuilding.position.x > transform.position.x);
            AdjustFacingDirection();

            Vector3 step = Vector3.MoveTowards(transform.position, targetBuilding.position, speed * Time.deltaTime);
            transform.position = step;
            yield return null;
        }

        animator.SetBool("isMoving", false);
        isMoving = false;
    }

    private void ActivateBuilding(Transform building)
    {
        Debug.Log($"Activating Building: {building.name}");
        var upgradeBuildingAnimatio = building.GetComponent<UpgradeBuildingAnimatio>();
        if (upgradeBuildingAnimatio != null)
        {
            // Find the index of the current building
            int buildingIndex = buildingList.buildingsToUpgrade.IndexOf(building);
            if (buildingIndex != -1)
            {
                // Ensure the index is valid and there is a corresponding upgrade time
                float upgradeTime = buildingList.buildingUpgradeTimeList[buildingIndex];
                Debug.Log("Activating upgrade animation with duration: " + upgradeTime);

                // Start the upgrade animation with the specific duration
                upgradeBuildingAnimatio.StartUpgradeAnimation(true, upgradeTime);
                upgradeBuildingAnimatio.OnUpgradeComplete += () => OnBuildingUpgradeComplete(building);
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



    private void OnBuildingUpgradeComplete(Transform building)
    {
        animator.SetBool("isBuilding", false);
        isUpgrading = false;

        buildingList.buildingsToUpgrade.Remove(building);
    }

    private void AdjustFacingDirection()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}