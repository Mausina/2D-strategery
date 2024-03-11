using System.Collections;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public float speed = 5f; // Movement speed of the builder.
    public float stamina = 10f; // Builder's stamina for continuous movement.
    public float staminaThreshold = 3f; // Minimum stamina required to start moving.
    public float patrolDistance = 5f; // Distance the builder will patrol around the camp.
    private BuildingList buildingList; // Reference to the BuildingList component.
    private Animator animator; // Reference to the animator component.
    private bool isMoving = false; // Indicates if the builder is currently moving.
    private bool isMovingRight = true; // Indicates if the builder is moving to the right (for sprite direction).
    private bool isUpgrading = false; // Indicates if the builder is currently upgrading a building.
    private Vector3 campPosition; // The position of the camp.

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
        GameObject camp = GameObject.FindGameObjectWithTag("Camp");
        if (camp != null)
        {
            campPosition = camp.transform.position;
        }
        else
        {
            Debug.LogError("Camp not found in the scene.");
            return;
        }
        StartCoroutine(MoveAndUpgradeBuildings());
    }

    private IEnumerator MoveAndUpgradeBuildings()
    {
        Vector3 patrolTarget = campPosition;
        bool isPatrolling = false;

        while (true)
        {
            if (buildingList.buildingsToUpgrade.Count > 0 && !isMoving && !isUpgrading && stamina > staminaThreshold)
            {
                isPatrolling = false;
                Transform targetBuilding = buildingList.buildingsToUpgrade[0];
                yield return StartCoroutine(MoveToPosition(targetBuilding.position)); // Adjusted for Vector3 position
                ActivateBuilding(targetBuilding);
                yield return new WaitUntil(() => !isMoving && !isUpgrading);
            }
            else if (buildingList.buildingsToUpgrade.Count == 0 && !isMoving)
            {
                if (!isPatrolling || Vector3.Distance(transform.position, patrolTarget) < 0.7f)
                {
                    patrolTarget = GetNewPatrolTarget();
                    isPatrolling = true;
                }
                yield return StartCoroutine(MoveToPosition(patrolTarget)); // Correctly call MoveToPosition for Vector3
            }

            // Stamina management
            if (isMoving)
            {
                stamina -= Time.deltaTime;
                if (stamina <= 0)
                {
                    yield return new WaitUntil(() => stamina > staminaThreshold);
                }
            }
            else
            {
                stamina = Mathf.Min(stamina + Time.deltaTime * 2, 10f);
            }

            yield return null;
        }
    }

    private Vector3 GetNewPatrolTarget()
    {
        // Randomly select a direction (left or right) and a distance within the patrol range
        float direction = Random.Range(0, 2) * 2 - 1; // -1 for left, 1 for right
        float distance = Random.Range(3f, patrolDistance);
        return campPosition + new Vector3(direction * distance, 0, 0);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        animator.SetBool("isMoving", true);
        while (Vector3.Distance(transform.position, targetPosition) > 0.7f)
        {
            isMovingRight = (targetPosition.x > transform.position.x);
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
        var upgradeBuildingAnimatio = building.GetComponent<UpgradeBuildingAnimatio>();
        if (upgradeBuildingAnimatio != null)
        {
            animator.SetBool("isBuilding", true);
            int buildingIndex = buildingList.buildingsToUpgrade.IndexOf(building);
            if (buildingIndex != -1)
            {
                float upgradeTime = buildingList.buildingUpgradeTimeList[buildingIndex];
                Debug.Log("Activating upgrade animation with duration: " + upgradeTime);
                StartCoroutine(RepeatUpgradeAnimation(upgradeTime, upgradeBuildingAnimatio));
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

    private IEnumerator RepeatUpgradeAnimation(float duration, UpgradeBuildingAnimatio upgradeAnim)
    {
        float timeLeft = duration;
        while (timeLeft > 0)
        {
            upgradeAnim.SetUpgradeAnimationState(true);
            yield return new WaitForSeconds(1); // Duration for each animation burst
            upgradeAnim.SetUpgradeAnimationState(false);
            timeLeft -= 1;
            yield return new WaitForSeconds(0); // Immediate continuation for demonstration, adjust as needed.
        }
        upgradeAnim.CompleteUpgrade();
    }

    private void AdjustFacingDirection()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}
