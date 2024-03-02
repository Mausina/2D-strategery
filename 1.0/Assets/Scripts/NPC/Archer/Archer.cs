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


    private void Start()
    {
        StartCoroutine(CheckTimeOfDay());
    }
    private void Update()
    {
        if (isNight && RallyPointManager.Instance.CurrentRallyPoint != null && !IsAtRallyPoint())
        {
            MoveTowardsRallyPoint();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
           // ShootArrow();
            CheckDetectionZone();
        }


    }
    private void DaytimeBehavior()
    {
        // Implement hunting behavior and other daytime activities
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
                //float angleOflaunch = 15f;
                //ShootArrowAtTarget(collider.transform.position, angleOflaunch);
                //ShootArrow();
                // Logic for encountering an animal (e.g., stop and shoot)
            }
            else if (collider.CompareTag("Enemy")) // Check if the detected object is an enemy
            {
                float angleOflaunch = 80f;
                ShootArrowAtTarget(collider.transform.position, angleOflaunch); // Shoot an arrow at the enemy
            }
        }
    }
    private void MoveArcher()
    {
        transform.Translate(Vector2.right * (isMovingRight ? 1 : -1) * speed * Time.deltaTime);
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
            Debug.Log($"Distance to Rally Point: {distance}, IsAtRallyPoint: {IsAtRallyPoint()}");
            // Continuously adjust facing direction based on movement direction
            isMovingRight = RallyPointManager.Instance.CurrentRallyPoint.position.x > transform.position.x;
            AdjustFacingDirection();
        }
    }
    private bool IsAtRallyPoint()
    {
        if (RallyPointManager.Instance.CurrentRallyPoint == null) return false;
        return Vector3.Distance(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position) < 0.2f;
    }
    /*
    private void FlipDirection()
    {
        isMovingRight = !isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    */
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
    private void ShootArrowAtTarget(Vector3 targetPosition, float angleOflaunch)
    {
        if (arrowPrefab && firePoint)
        {
            // The starting point of the arrow
            Vector3 startPosition = firePoint.position;

            // The gravity value; assuming Physics2D.gravity.y is being used
            float gravity = Physics2D.gravity.magnitude;

            // The desired angle of launch
            float angle = angleOflaunch; // You can adjust this angle as needed

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
        }
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