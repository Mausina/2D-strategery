using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchZone : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public List<Collider2D> detectedCollidersTree = new List<Collider2D>();
    public List<Collider2D> detectedCollidersWall = new List<Collider2D>();
    private bool isSafeZoneDetected = false;

    private float checkInterval = 1f; // Time interval between checks
    private int requiredTreeCount = 5;
    private float expansionStep = 0.5f;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(AdjustColliderRoutine());
    }

    IEnumerator AdjustColliderRoutine()
    {
        while (true)
        {
            if (ShouldExpand())
            {
                yield return StartCoroutine(ExpandZone());
            }

            AdjustForNewObstacles();

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator ExpandZone()
    {
        // Expand until the required number of trees are detected, even if a wall is present.
        while (detectedCollidersTree.Count < requiredTreeCount && !isSafeZoneDetected)
        {
            // We expand regardless of the presence of a wall, so we remove the wall count check
            boxCollider.size += new Vector2(expansionStep, 0);

            // We only move the offset if no wall is detected to the right side of the collider
            if (!IsWallOnRightSide())
            {
                boxCollider.offset += new Vector2(expansionStep / 2, 0);
            }

            yield return new WaitForFixedUpdate();

            // If a wall is detected on the right during expansion, adjust the collider once
            if (IsWallOnRightSide())
            {
                AdjustColliderToWall();
                break; // Break the loop if a wall is detected to avoid expanding into the wall
            }
        }
    }

    private bool ShouldExpand()
    {
        return detectedCollidersTree.Count < requiredTreeCount && !isSafeZoneDetected;
    }

    private void AdjustForNewObstacles()
    {
        if (detectedCollidersWall.Count > 0)
        {
            var rightmostWall = GetRightmostWall();

            if (rightmostWall != null)
            {
                AdjustCollider(rightmostWall);
            }
        }
    }

    private Collider2D GetRightmostWall()
    {
        return detectedCollidersWall
            .OrderByDescending(wall => wall.transform.position.x)
            .FirstOrDefault(wall => wall != null && wall.isActiveAndEnabled);
    }
    private bool IsWallOnRightSide()
    {
        // Assuming walls can only appear from the right side for this check
        return detectedCollidersWall.Any(wall => wall.transform.position.x > boxCollider.bounds.max.x);
    }

    private void AdjustColliderToWall()
    {
        // Find the rightmost wall that is inside the collider bounds
        Collider2D rightmostWallInsideBounds = detectedCollidersWall
            .OrderByDescending(wall => wall.transform.position.x)
            .FirstOrDefault();

        if (rightmostWallInsideBounds != null)
        {
            float wallPositionX = rightmostWallInsideBounds.transform.position.x;
            float colliderRightBoundary = boxCollider.bounds.max.x;

            // If the wall is within the bounds of the collider, adjust the collider
            if (wallPositionX < colliderRightBoundary)
            {
                // Calculate the new width of the collider based on the wall's position
                float newWidth = wallPositionX - boxCollider.bounds.min.x;
                boxCollider.size = new Vector2(newWidth, boxCollider.size.y);
                // Since we're only adjusting the size, no change to offset is necessary
            }
        }
    }
    private void AdjustCollider(Collider2D rightmostWall)
    {
        Physics2D.SyncTransforms(); // Sync transforms before making adjustments

        float wallPositionX = rightmostWall.transform.position.x;
        float colliderLeftBoundary = transform.position.x + boxCollider.offset.x - boxCollider.size.x / 2;

        if (wallPositionX > colliderLeftBoundary)
        {
            float newWidth = wallPositionX - colliderLeftBoundary;
            boxCollider.size = new Vector2(newWidth, boxCollider.size.y);
            // Sync transforms after making adjustments
            Physics2D.SyncTransforms();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree"))
        {
            detectedCollidersTree.Add(collision);
        }
        else if (collision.CompareTag("Wall"))
        {
            detectedCollidersWall.Add(collision);
        }
        else if (collision.CompareTag("SafeZone"))
        {
            isSafeZoneDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree"))
        {
            detectedCollidersTree.Remove(collision);
        }
        else if (collision.CompareTag("Wall"))
        {
            detectedCollidersWall.Remove(collision);
        }
        else if (collision.CompareTag("SafeZone"))
        {
            isSafeZoneDetected = false;
        }
    }
}

