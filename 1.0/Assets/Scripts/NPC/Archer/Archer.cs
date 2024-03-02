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

        /*
        else if (RallyPointManager.Instance.CurrentRallyPoint == null)
        {
            MoveArcher();
        }
        */
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
        return Vector3.Distance(transform.position, RallyPointManager.Instance.CurrentRallyPoint.position) < 1.5f;
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
                    FlipDirection();
                    break;
                }
            }
            else if (collider.CompareTag("Animal"))
            {
                // Logic for encountering an animal (e.g., stop and shoot)
            }
        }
    }
    private void MoveArcher()
    {
        transform.Translate(Vector2.right * (isMovingRight ? 1 : -1) * speed * Time.deltaTime);
    }

    private void FlipDirection()
    {
        isMovingRight = !isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
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
    private void ShootArrow2()
    {
        if (arrowPrefab && firePoint)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 targetPosition = player.transform.position;
                Vector3 startPosition = firePoint.position;
                float gravity = Physics2D.gravity.magnitude;

                // Increase the launch angle for a higher trajectory
                float desiredLaunchAngle = 80f; // Example: Increase to 60 degrees for a higher shot

                // Calculate the distance to the target (ignoring height for now)
                float distanceToTarget = Vector2.Distance(new Vector2(startPosition.x, startPosition.y), new Vector2(targetPosition.x, targetPosition.y));

                // Adjust the launch force to ensure the arrow can reach the target at a higher launch angle
                float adjustedLaunchForce = CalculateLaunchForce(desiredLaunchAngle, distanceToTarget, gravity);

                // Set the arrow's position and rotation
                GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, desiredLaunchAngle));
                Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Calculate the launch velocity vector with the adjusted force and angle
                    Vector2 launchVelocity = new Vector2(adjustedLaunchForce * Mathf.Cos(desiredLaunchAngle * Mathf.Deg2Rad), adjustedLaunchForce * Mathf.Sin(desiredLaunchAngle * Mathf.Deg2Rad));
                    rb.velocity = launchVelocity;
                }
                else
                {
                    Debug.LogError("Arrow prefab is missing Rigidbody2D component.");
                }
            }
            else
            {
                Debug.LogError("Player object not found.");
            }
        }
    }

    // Helper method to calculate the required launch force for a given angle and distance
    private float CalculateLaunchForce(float angle, float distance, float gravity)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        float force = Mathf.Sqrt((gravity * distance * distance) / (distance * Mathf.Sin(2 * angleRad)));
        return force;
    }


}