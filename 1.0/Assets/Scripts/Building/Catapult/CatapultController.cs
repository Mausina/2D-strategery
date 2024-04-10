using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultController : MonoBehaviour
{
    public List<GameObject> shellPrefab;
    public Transform firePoint; // Assign the position from which arrows are fired
    public float launchForce = 20f; // Adjustable force to apply for arrow shooting
    public float speed = 0.5f; // Reduced speed for a slow and deliberate pace
    public DetectionZone detectionZone;
    private float fireDelay = 1f;
    public float shootCooldownSeconds; // Time in seconds between shots
    private float lastShotTime = 0f; // When the last shot was fired
    public List<GameObject> wheels;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Constrain the Rigidbody2D to prevent flying off into space and rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        CheckDetectionZone();
    }
    /// <summary>
    /// ////////////////// FixedUpdate() for Test 
    /// </summary>
    void FixedUpdate()
    {
        // Handle left and right movement in FixedUpdate for physics consistency
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1; // Move left
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1; // Move right
        }

        if (moveInput != 0)
        {
            MoveCatapult(moveInput);
            RotateWheels(moveInput < 0); // Rotate wheels based on direction
        }
    }

    private void MoveCatapult(float direction)
    {
        // Adjust this force as needed to simulate the wheel push
        float moveForce = 10f;

        // Apply force at the position of each wheel
        foreach (GameObject wheel in wheels)
        {
            if (wheel != null)
            {
                // You may want to add force at the wheel's position to simulate the wheel pushing
                rb.AddForceAtPosition(Vector2.right * direction * moveForce, wheel.transform.position);
            }
        }
    }



    // Modified RotateWheels method to include direction
    private void RotateWheels(bool isLeft)
    {
        float directionMultiplier = isLeft ? -1 : 1;
        foreach (GameObject wheel in wheels)
        {
            if (wheel != null)
            {
                float rotationAmount = speed * Time.deltaTime * 360 * directionMultiplier;
                wheel.transform.Rotate(0, 0, rotationAmount, Space.Self);
            }
        }
    }

    private void CheckDetectionZone()
    {
        List<Transform> enemies = new List<Transform>();

        foreach (Collider2D collider in detectionZone.detectedColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemies.Add(collider.transform);
            }

        }
        // Check if there are any enemies detected
        if (enemies.Count > 0 && Time.time >= lastShotTime + shootCooldownSeconds)
        {
            Transform targetEnemy = enemies[Random.Range(0, enemies.Count)];

            float angleOfLaunch = 72f; // Adjust as necessary for your game
            ShootArrowAtTarget(targetEnemy.position, angleOfLaunch);
            lastShotTime = Time.time;

        }
    }

    private void ShootArrowAtTarget(Vector3 targetPosition, float angleOflaunch)
    {
        if (firePoint)
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
        float angleVariation = Random.Range(-10f, 10f);
        float angle = angleOfLaunch + angleVariation; // Adjusted angle with randomness
        float randomFactor = Random.Range(0.9f, 1.1f);
        Vector2 velocity = CalculateVelocity(targetPosition, startPosition, gravity * randomFactor, angle); // Apply randomness in gravity effect
        GameObject arrow = Instantiate(shellPrefab[0], startPosition, Quaternion.identity);
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