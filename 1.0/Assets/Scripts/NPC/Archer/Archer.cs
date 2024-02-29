using UnityEngine;
using System;

public class Archer : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform defenseWall; // Assign this in the Inspector to specify the wall
    public GameObject arrowPrefab;
    public Transform shootPoint;
    private float shootCooldown = 2f;
    private float shootTimer = 0f;
    private bool isNight = false;
    private Vector2 defensePosition;
    public LayerMask enemyLayer;
    public float detectionRadius = 5f;
    private Transform targetEnemy = null;
    public float patrolRange = 2f; // Distance the archer patrols from the defense position
    private Vector2 patrolTarget;
    private bool isPatrolling = false;
    private float patrolCooldown = 5f; // Time between patrol movements
    private float patrolTimer = 0f;

    void Start()
    {
        // Your existing start logic...
        if (defenseWall != null)
        {
            CalculateDefensePosition();
            patrolTarget = defensePosition; // Initialize patrol target to defense position
        }
    }

    void Update()
    {
        // Your existing update logic...
        TimeCheck();
        shootTimer += Time.deltaTime;
        patrolTimer += Time.deltaTime;

        if (isNight)
        {
            // Move to the defense position at night
            MoveToPosition(defensePosition);
        }
        else
        {
            // During the day, remain alert but stay near the defense position
            StayAlert();
        }

        if (shootTimer >= shootCooldown && targetEnemy != null)
        {
            ShootArrow(targetEnemy.position);
            shootTimer = 0;
        }
    }

    void FixedUpdate()
    {
        DetectEnemies();
    }

    private void TimeCheck()
    {
        // Simple placeholder for time check; replace with your WorldTime system as needed
        var time = DateTime.Now.Hour;
        isNight = time >= 21 || time < 6;
    }

    private void CalculateDefensePosition()
    {
        // Stand on the right side of the wall, a fixed distance away
        defensePosition = new Vector2(defenseWall.position.x + 1.5f, defenseWall.position.y);
    }

    private void MoveToPosition(Vector2 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
    }

    private void StayAlert()
    {
        if (!isPatrolling && patrolTimer >= patrolCooldown)
        {
            // Choose a new patrol target within range of the defense position
            float patrolX = UnityEngine.Random.Range(defensePosition.x - patrolRange, defensePosition.x + patrolRange);
            patrolTarget = new Vector2(patrolX, defensePosition.y);
            isPatrolling = true;
            patrolTimer = 0f; // Reset patrol timer
        }
        else if (isPatrolling)
        {
            // Move towards the patrol target
            MoveToPosition(patrolTarget);

            if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
            {
                isPatrolling = false; // Stop patrolling once the target is reached
            }
        }
    }

    private void DetectEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                targetEnemy = hit.transform;
                return; // Target the first enemy within range
            }
        }
        targetEnemy = null; // Reset if no enemies are detected
    }

    private void ShootArrow(Vector2 targetPosition)
    {
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        // Direct the arrow towards the target
        arrow.GetComponent<Rigidbody2D>().velocity = (targetPosition - (Vector2)shootPoint.position).normalized * 10f; // Adjust speed as necessary
    }
}
