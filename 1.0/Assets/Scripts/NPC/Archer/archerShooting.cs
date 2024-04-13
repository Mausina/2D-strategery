using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Archer
{
    public class archerShooting : MonoBehaviour
    {
        public GameObject arrowPrefab; // Assign your arrow prefab in the inspector
        public Transform firePoint; // Assign the position from which arrows are fired
        public float launchForce = 20f; // Adjustable force to apply for arrow shooting
        public float launchAngle = 45f; // Angle at which the arrow is launched  
        public DetectionZone detectionZone;
        private bool isMovingRight = true;
        private float fireDelay = 1f;
        public float shootCooldownSeconds; // Time in seconds between shots
        private float lastShotTime = 0f; // When the last shot was fired
        public float deviationAngle = 5f;
        private Animator animator;
        ArcherController archerController;


        private void Start()
        {
            animator = GetComponent<Animator>();
            lastShotTime = Time.time; // Initialize lastShotTime with the current time
        }

        private void Awake()
        {
            archerController = GetComponent<ArcherController>();
        }

        private void AdjustFacingDirection(bool shouldFaceRight)
        {
            // Set isMovingRight based on the parameter
            isMovingRight = shouldFaceRight;

            // Adjust the facing direction of the NPC based on isMovingRight
            Vector3 localScale = transform.localScale;
            localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
        public void CheckDetectionZone()
        {
            List<Transform> enemies = new List<Transform>();

            foreach (Collider2D collider in detectionZone.detectedColliders)
            {
                if (collider.CompareTag("Animal"))
                {
                    Transform targetEnemy = enemies[Random.Range(0, enemies.Count)];
                    bool shouldFaceRight = targetEnemy.position.x > transform.position.x;
                    AdjustFacingDirection(shouldFaceRight);
                    ShootArrowDirectly(targetEnemy.position);
                    lastShotTime = Time.time;
                }
                else if (collider.CompareTag("Enemy"))
                {
                    archerController.PauseMovement();
                    enemies.Add(collider.transform);
                }

            }
            // Check if there are any enemies detected
            if (enemies.Count > 0 && Time.time >= lastShotTime + shootCooldownSeconds)
            {
                Transform targetEnemy = enemies[Random.Range(0, enemies.Count)];
                bool shouldFaceRight = targetEnemy.position.x > transform.position.x;
                AdjustFacingDirection(shouldFaceRight);

                // Define a LayerMask for the Wall layer (adjust the layer number as needed)
                LayerMask wallLayer = LayerMask.GetMask("Wall");

                // Adjust the Raycast to use the LayerMask
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, (targetEnemy.position - firePoint.position).normalized, Mathf.Infinity, wallLayer);
                Debug.DrawLine(firePoint.position, targetEnemy.position, Color.red, 5f);


                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {

                    // If there's a wall, shoot over it
                    animator.SetBool("isMove", false);
                    animator.SetBool("isRun", false);
                    float angleOfLaunch = 72f; // Adjust as necessary for your game
                    ShootArrowAtTarget(targetEnemy.position, angleOfLaunch);
                    lastShotTime = Time.time;
                }
                else
                {
                    // No wall detected, proceed with direct shot
                    ShootArrowDirectly(targetEnemy.position);
                    lastShotTime = Time.time;
                }
            }
        }

        private void ShootArrowDirectly(Vector3 targetPosition)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootingDirection = (targetPosition - firePoint.position).normalized;

                // Randomize the shooting direction slightly for inaccuracy
                float deviation = UnityEngine.Random.Range(-deviationAngle, deviationAngle); // Define 'deviationAngle' as the max deviation you want in degrees
                shootingDirection = Quaternion.Euler(0, 0, deviation) * shootingDirection;

                rb.velocity = shootingDirection * launchForce;
                float angleDegrees = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
                arrow.transform.rotation = Quaternion.AngleAxis(angleDegrees, Vector3.forward);

                // After shooting, resume movement if you have such logic
                archerController.ResumeMovement();
            }
        }



        private void ShootArrowAtTarget(Vector3 targetPosition, float angleOflaunch)
        {
            if (arrowPrefab && firePoint)
            {

                animator.SetBool("isFire", true);
                archerController.PauseMovement();
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
            archerController.ResumeMovement();
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
}