using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using WorldTimeSystem;

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
    public float shootCooldownSeconds = 1f; // Time in seconds between shots
    private float lastShotTime = 0f; // When the last shot was fired
    public float chanceToIdle = 0.1f; // Chance to idle every 10 seconds
    public float chanceToTurnAround = 0.3f; // Chance to turn around after idling
    public float idleDuration = 3f; // How long to idle
    private bool isIdling = false;
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

            if (isNight && RallyPointManager.Instance.CurrentRallyPoint != null && !IsAtRallyPoint())
            {
                MoveTowardsRallyPoint();
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
        if (isIdling) return;

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

        // Continue moving the archer
        MoveArcher();
    }

    private IEnumerator IdleRoutine()
    {
        // Start idling
        isIdling = true;
        // Trigger idle animation
        // animator.SetBool("IsIdling", true);

        // Wait for the duration of the idle
        yield return new WaitForSeconds(idleDuration);

        // Optionally turn around with a certain chance
        if (UnityEngine.Random.value < chanceToTurnAround)
        {
            AdjustFacingDirection();
        }

        // Stop idling
        isIdling = false;
        // Trigger walking animation
        // animator.SetBool("IsIdling", false);
    }

    private void MoveArcher()
    {
        // Only move if not at rally point and not idling
        if (!IsAtRallyPoint() && !isIdling)
        {
            transform.Translate(Vector2.right * (isMovingRight ? 1 : -1) * speed * Time.deltaTime);
            animator.SetBool("isMove", true);
            // Example for isRun - this is just an example condition
            animator.SetBool("isRun", speed > 1.0f); // Assuming 'speed > 1.0f' indicates running
        }
        else
        {
            animator.SetBool("isMove", false);
            animator.SetBool("isRun", false);
        }
    }

    private bool IsAtRallyPoint()
    {
        if (RallyPointManager.Instance.CurrentRallyPoint == null) return false;
        bool atRallyPoint = Vector3.Distance(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position) < 0.5f;

        // If at rally point and it's daytime, start idling
        if (atRallyPoint && !isNight && !isIdling)
        {
            StartCoroutine(IdleRoutine());
        }

        return atRallyPoint;
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
                float angleOflaunch = 15f;
                ShootArrowAtTarget(collider.transform.position, angleOflaunch);
                // Logic for encountering an animal (e.g., stop and shoot)
            }
            else if (collider.CompareTag("Enemy") && Time.time >= lastShotTime + shootCooldownSeconds)
            {
                if (IsAtRallyPoint() == true )
                {
                    animator.SetBool("isMove", false);
                    animator.SetBool("isRun", false);
                    float angleOflaunch = 72f;
                    ShootArrowAtTarget(collider.transform.position, angleOflaunch); // Shoot an arrow at the enemy
                    lastShotTime = Time.time; // Reset the last shot time
                    break; // Add this if you want to shoot only one arrow per check
                }
                else
                {
                   // animator.SetBool("isFire", true);
                   // animator.SetBool("isMove", false);
                   // animator.SetBool("isRun", false);
                    ShootArrow();
                   // float angleOflaunch = 5f;
                   // ShootArrowAtTarget(collider.transform.position, angleOflaunch);
                    lastShotTime = Time.time; // Reset the last shot time
                    break; // Add this if you want to shoot only one arrow per chec
                }
            }
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
                if (!IsAtRallyPoint())
                {
                    MoveTowardsRallyPoint();
                }
            }
            else
            {
                // Implement daytime behavior here
                DaytimeBehavior();
            }
            yield return null; // This ensures the check runs continuously
        }
    }
    private void MoveTowardsRallyPoint()
    {
        if (RallyPointManager.Instance.CurrentRallyPoint != null)
        {
            Vector3 direction = RallyPointManager.Instance.CurrentRallyPoint.position - transform.position;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position, step);
            float distance = Vector3.Distance(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position);
           // Debug.Log($"Distance to Rally Point: {distance}, IsAtRallyPoint: {IsAtRallyPoint()}");
            // Continuously adjust facing direction based on movement direction
            isMovingRight = RallyPointManager.Instance.CurrentRallyPoint.position.x > transform.position.x;
            AdjustFacingDirection();
        }
    }
    /*
    private bool IsAtRallyPoint()
    {
        if (RallyPointManager.Instance.CurrentRallyPoint == null) return false;
        return Vector3.Distance(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position) < 0.2f;
    }
    */
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
        // The starting point of the arrow
        Vector3 startPosition = firePoint.position;

        // The gravity value; assuming Physics2D.gravity.y is being used
        float gravity = Physics2D.gravity.magnitude;

        // The desired angle of launch
        float angle = angleOfLaunch; // You can adjust this angle as needed

        // Calculate the velocity needed to throw the object to the target point
        Vector2 velocity = CalculateVelocity(targetPosition, startPosition, gravity, angle);

        // Instantiate the arrow and set its velocity
        GameObject arrow = Instantiate(arrowPrefab, startPosition, Quaternion.identity); // Use Quaternion.identity for initial rotation
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = velocity;
            float angleDegrees = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angleDegrees, Vector3.forward); // Adjust rotation based on velocity
        }
        //StartCoroutine(ResetFireState());
        Debug.Log($"Trying to shoot: Time={Time.time}, LastShot={lastShotTime}, Cooldown={shootCooldownSeconds}, CanShoot={Time.time >= lastShotTime + shootCooldownSeconds}");

        // Make sure to reset the isFire flag after the delay if it's not being reset elsewhere
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
    private void ShootArrow()
    {
        if (arrowPrefab && firePoint)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(firePoint.right * launchForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("Arrow prefab is missing Rigidbody2D component.");
            }
        }
    }



}