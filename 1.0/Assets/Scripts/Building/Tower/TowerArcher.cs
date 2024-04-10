using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerArcher : MonoBehaviour
{
    public GameObject arrowPrefab; // Assign your arrow prefab in the inspector
    public Transform firePoint; // Assign the position from which arrows are fired
    public float launchForce = 20f; // Adjustable force to apply for arrow shooting
    public float shootCooldownSeconds; // Time in seconds between shots
    private float lastShotTime = 0f; // When the last shot was fired
    public DetectionZone detectionZone; // Assign the detection zone component
    public LayerMask layerForArrow; // Assign the layer for the arrow in the inspector
    private bool isFacingRight = true; // Initial direction the archer is facing

    void Update()
    {
        CheckDetectionZone();
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
        // Check if there are any enemies detected and if cooldown has passed
        if (enemies.Count > 0 && Time.time >= lastShotTime + shootCooldownSeconds)
        {
            Transform closestEnemy = GetClosestEnemy(enemies);
            bool shouldFaceRight = closestEnemy.position.x > transform.position.x;
            AdjustFacingDirection(shouldFaceRight);
            ShootArrow(closestEnemy); // Now passing closestEnemy as an argument
            lastShotTime = Time.time;
        }
    }

    private Transform GetClosestEnemy(List<Transform> enemies)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (Transform enemy in enemies)
        {
            float distance = (enemy.position - position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void AdjustFacingDirection(bool shouldFaceRight)
    {
        if (isFacingRight != shouldFaceRight)
        {
            // Flip the archer's direction by scaling the x-axis negatively or positively
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
            isFacingRight = shouldFaceRight;
        }
    }

    private void ShootArrow(Transform targetEnemy)
    {
        if (arrowPrefab && firePoint)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, isFacingRight ? 0 : 180));
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Convert the LayerMask to a single layer index
                int layerIndex = LayerMaskToLayerIndex(layerForArrow);
                arrow.layer = layerIndex;

                // Calculate the direction and apply force as previously described
                Vector2 directionToEnemy = (targetEnemy.position - firePoint.position).normalized;
                float angleVariation = Random.Range(-5f, 5f);
                Vector2 shootingDirection = (Quaternion.Euler(0, 0, angleVariation) * directionToEnemy).normalized;
                float forceVariation = Random.Range(-3f, 3f);
                rb.AddForce(shootingDirection * (launchForce + forceVariation), ForceMode2D.Impulse);
                rb.drag = 1f; // Adjust this value as needed
            }
            else
            {
                Debug.LogError("Arrow prefab is missing Rigidbody2D component.");
            }
        }
    }

    private int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        int layerIndex = 0;
        int layer = layerMask.value;
        while (layer > 1)
        {
            layer = layer >> 1;
            layerIndex++;
        }
        return layerIndex;
    }
}
