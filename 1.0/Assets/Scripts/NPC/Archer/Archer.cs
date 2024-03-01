using System.Security.Cryptography;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign your arrow prefab in the inspector
    public Transform firePoint; // Assign the position from which arrows are fired
    public AnimationCurve curve; // Assign an Animation Curve in the inspector
    public float launchForce = 20f; // Adjustable force to apply for arrow shooting
    public float launchAngle = 45f; // Angle at which the arrow is launched
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) // Trigger the shooting with the U key
        {
            ShootArrow2();
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
