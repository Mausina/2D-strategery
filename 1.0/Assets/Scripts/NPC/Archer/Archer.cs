using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using WorldTimeSystem;
using Random = UnityEngine.Random;
public class ArcherController : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign your arrow prefab in the inspector
    public Transform firePoint; // Assign the position from which arrows are fired
    public float launchForce = 20f; // Adjustable force to apply for arrow shooting
    public float launchAngle = 45f; // Angle at which the arrow is launched  
    public float speed = 0.5f; // Reduced speed for a slow and deliberate pace
    public DetectionZone detectionZone;
    private bool isMovingRight = true;
    private bool isNight = false;
    private float fireDelay =1f;
    public float shootCooldownSeconds; // Time in seconds between shots
    private float lastShotTime = 0f; // When the last shot was fired
    public float chanceToIdle = 0.1f; // Chance to idle every 10 seconds
    public float chanceToTurnAround = 0.3f; // Chance to turn around after idling
    public float idleDuration = 3f; // How long to idle
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(CheckTimeOfDay());
        lastShotTime = Time.time; // Initialize lastShotTime with the current time
    }


    private void Update()
    {
        // Perform actions based on time of day
        if (WorldTimeSystem.WorldTime.Instance != null)
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            bool isNight = currentTime.Hours < 6 || currentTime.Hours >= 18;

            if (isNight && RallyPointManager.Instance.CurrentRallyPoint != null)
            {
            }
            else if (!isNight)
            {
                DaytimeBehavior();
            }
        }
        CheckDetectionZone();
    }

    private float idleCheckTimer = 10f; // Timer to check for idle

    private void DaytimeBehavior()
    {
        // If the archer is idling, don't do anything else

        // Decrease the timer
        idleCheckTimer -= Time.deltaTime;

        // Every 10 seconds, check if the archer should idle
        if (idleCheckTimer <= 0f)
        {
            idleCheckTimer = 10f; // Reset the timer

            // Randomly decide to idle
            if (UnityEngine.Random.value < chanceToIdle)
            {
                StartCoroutine(IdleRoutine());
            }
        }


    }

    private IEnumerator IdleRoutine()
    {

        yield return new WaitForSeconds(idleDuration);

        // Optionally turn around with a certain chance
        if (UnityEngine.Random.value < chanceToTurnAround)
        {
            AdjustFacingDirection();
        }

    }


    IEnumerator CheckTimeOfDay()
    {
        while (true)
        {
            // Replace this with your actual day/night cycle check
            isNight = (Time.time % 24 < 12);

            if (isNight)
            {

            }
            else
            {
                // Implement daytime behavior here
                DaytimeBehavior();
            }
            yield return null; // This ensures the check runs continuously
        }
    }




    private void AdjustFacingDirection()
    {
        // Adjust the facing direction of the NPC based on isMovingRight
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
    private void CheckDetectionZone()
    {
        List<Transform> enemies = new List<Transform>();
        int treeCount = 0;

        foreach (Collider2D collider in detectionZone.detectedColliders)
        {
            if (collider.CompareTag("Tree"))
            {
                treeCount++;
                if (treeCount >= 3)
                {
                    AdjustFacingDirection();
                    break;
                }
            }
            else if (collider.CompareTag("Animal"))
            {
                ShootArrow();
                lastShotTime = Time.time;
            }
            else if (collider.CompareTag("Enemy"))
            {
                enemies.Add(collider.transform);
            }

        }
        // Check if there are any enemies detected
        if (enemies.Count > 0 && Time.time >= lastShotTime + shootCooldownSeconds)
        {
            // Randomly select one enemy from the list if more than one enemy is present
            Transform targetEnemy = enemies[Random.Range(0, enemies.Count)];

            // Proceed to target the selected enemy
     
                animator.SetBool("isMove", false);
                animator.SetBool("isRun", false);
                float angleOflaunch = 72f; // Example angle, adjust as needed
                ShootArrowAtTarget(targetEnemy.position, angleOflaunch);
                lastShotTime = Time.time;
            

                ShootArrow();
                lastShotTime = Time.time;
            
        }
    }

    private void ShootArrow()
    {
        if (arrowPrefab && firePoint)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float angleVariation = Random.Range(-2f, 2f);
                Vector2 adjustedDirection = (Quaternion.Euler(0, 0, 5 + angleVariation) * firePoint.right).normalized;
                float forceVariation = Random.Range(-1f, 1f);
                rb.AddForce(adjustedDirection * (launchForce + forceVariation), ForceMode2D.Impulse);

                // Update the lastShotTime and randomly adjust shootCooldownSeconds
                lastShotTime = Time.time;
                shootCooldownSeconds = Random.Range(1f, 2.5f); // Random cooldown between 1 and 2.5 seconds
            }
            else
            {
                Debug.LogError("Arrow prefab is missing Rigidbody2D component.");
            }
        }
    }


    private void ShootArrowAtTarget(Vector3 targetPosition, float angleOflaunch)
    {
        if (arrowPrefab && firePoint)
        {
            animator.SetBool("isFire", true);
            StartCoroutine(FireArrowAfterDelay(targetPosition, angleOflaunch, fireDelay));
           
        }
    }
    IEnumerator FireArrowAfterDelay(Vector3 targetPosition, float angleOfLaunch, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Here, the arrow is about to be fired...
        Vector3 startPosition = firePoint.position;
        float gravity = Physics2D.gravity.magnitude;
        float angleVariation = Random.Range(-3f, 3f);
        float angle = angleOfLaunch + angleVariation; // Adjusted angle with randomness

        Vector2 velocity = CalculateVelocity(targetPosition, startPosition, gravity, angle);
        GameObject arrow = Instantiate(arrowPrefab, startPosition, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = velocity;
            float angleDegrees = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angleDegrees, Vector3.forward);
        }

        // Arrow has been fired; now adjust the shoot cooldown for the next shot.
        lastShotTime = Time.time;
        shootCooldownSeconds = Random.Range(1f, 2.5f); // Set new random cooldown between 1 and 2.5 seconds

        animator.SetBool("isFire", false);
    }



    private Vector2 CalculateVelocity(Vector3 target, Vector3 source, float gravity, float angle)
    {
        // Distance between target and source
        float distance = Vector2.Distance(target, source);

        // Convert angle to radians
        float angleRad = angle * Mathf.Deg2Rad;

        // Calculate velocity
        float velocity = Mathf.Sqrt(distance * gravity / Mathf.Sin(2 * angleRad));

        // Get velocity components in 2D
        float velocityX = velocity * Mathf.Cos(angleRad);
        float velocityY = velocity * Mathf.Sin(angleRad);

        // Adjust direction based on target and source positions
        Vector2 direction = (target - source).normalized;
        float sign = (target.x > source.x) ? 1f : -1f;

        // Return velocity vector
        return new Vector2(velocityX * sign, velocityY) * direction.magnitude;
    }



}