using System;
using System.Collections;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 10f; // Speed when running to a new building
    private BuildingList buildingList;
    private Animator animator;
    private bool isMoving = false;
    private Collider2D targetSafeZoneCollider = null;
    private bool isMovingRight = true;
    private GameObject currentBuilding = null; // Current building to work on
    private Vector3 buildingPosition; // The position of the building to move towards
    private bool isNight = false;
    private bool isRunning = false;

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

        FindNearestSafeZoneCollider();
        CheckStartPosition();
    }

    private void Update()
    {
        CheckForNight();

        if (isMoving)
        {
            AdjustFacingDirection();
        }
        HandleBuilderBehavior();
    }
    private void CheckForNight()
    {
        if (WorldTimeSystem.WorldTime.Instance != null)
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            isNight = currentTime.Hours < 6 || currentTime.Hours >= 18;
        }
    }
    private void HandleBuilderBehavior()
    {
        
    }
    public void AssignToBuild(GameObject building)
    {
        StopAllCoroutines();
        Debug.Log("It WORKS!!!!!!");
    }


    public void ResetBuilder()
    {

    }

    private void FindNearestSafeZoneCollider()
    {
        GameObject[] safeZones = GameObject.FindGameObjectsWithTag("SafeZone");
        float closestDistance = Mathf.Infinity;
        foreach (GameObject zone in safeZones)
        {
            Collider2D zoneCollider = zone.GetComponent<Collider2D>();
            if (zoneCollider != null)
            {
                float distance = Vector3.Distance(transform.position, zoneCollider.bounds.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetSafeZoneCollider = zoneCollider;
                }
            }
        }
    }
    private void CheckStartPosition()
    {
        if (targetSafeZoneCollider != null && targetSafeZoneCollider.OverlapPoint(transform.position))
        {
            StartMoveRoutineInsideSafeZone();
        }
            StartMovingToSafeZone();
        
    }

    private void StartMovingToSafeZone()
    {
        isMovingRight = transform.position.x < targetSafeZoneCollider.bounds.center.x;
        AdjustFacingDirection();
        StartCoroutine(MoveTowardsSafeZone(targetSafeZoneCollider.bounds.min));
    }

    IEnumerator MoveTowardsSafeZone(Vector3 target)
    {
        isMoving = true;
        animator.SetBool("isMoving", isMoving);

        while (Vector3.Distance(transform.position, target) > 0.5f)
        {

            MoveTowards(target, speed);

            yield return null;
        }

        StopMoving();
        // Instead of starting the MoveRoutine immediately, wait for a second.
        StartCoroutine(WaitAndStartMoveRoutineInsideSafeZone());
    }

    private IEnumerator WaitAndStartMoveRoutineInsideSafeZone()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        StartMoveRoutineInsideSafeZone();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone"))
        {
            // Stop all movement and start the routine
            StopAllCoroutines(); // Stop any current movement coroutines
            StartCoroutine(PauseAndStartMoveRoutine());
        }
    }
    private IEnumerator PauseAndStartMoveRoutine()
    {
        isMoving = false;
        animator.SetBool("isMoving", isMoving);
        yield return new WaitForSeconds(1f); // Wait for a second

        // Now that we've paused, start moving within the safezone
        StartMoveRoutineInsideSafeZone();
    }
    private void StartMoveRoutineInsideSafeZone()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        float originalSpeed = speed;

        while (true)
        {
            Vector2 newTargetPointWithinSafeZone = GetRandomPointAlongBottomOfCollider(targetSafeZoneCollider);
            isMovingRight = transform.position.x < newTargetPointWithinSafeZone.x;
            AdjustFacingDirection();

            isMoving = true;
            animator.SetBool("isMoving", isMoving);
            while (Vector3.Distance(transform.position, new Vector3(newTargetPointWithinSafeZone.x, transform.position.y, 0)) > 0.5f)
            {
                MoveTowards(new Vector3(newTargetPointWithinSafeZone.x, transform.position.y, 0), originalSpeed);
                yield return null;
            }

            isMoving = false;
            animator.SetBool("isMoving", isMoving);
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 10));
        }
    }
    private Vector2 GetRandomPointAlongBottomOfCollider(Collider2D collider)
    {
        float x = UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        float y = collider.transform.position.y; // Set Y to the SafeZone's Y position
        return new Vector2(x, y);
    }

    private void MoveTowards(Vector3 target, float moveSpeed)
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        // Facing direction should be updated immediately after setting isMovingRight
        AdjustFacingDirection();
    }

    void StopMoving()
    {
        isMoving = false;
        animator.SetBool("isMoving", isMoving);
    }

    void AdjustFacingDirection()
    {
        animator.SetBool("isMoving", isMoving);
        if (isMoving) // Only adjust the direction if the guard is supposed to be moving
        {
            Vector3 localScale = transform.localScale;
            localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }


}