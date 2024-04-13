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

    // References to GameObjects representing the corners of the collider
    public GameObject topLeftCorner;
    public GameObject topRightCorner;
    public GameObject bottomLeftCorner;
    public GameObject bottomRightCorner;

    private float checkInterval = 1f; // Time interval between checks
    private int requiredTreeCount = 5;
    private float expansionStep = 0.5f;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        DontDestroyOnLoad(gameObject);

        // Initialize corner GameObjects
        InitializeCorners();

        StartCoroutine(AdjustColliderRoutine());
    }

    private void InitializeCorners()
    {
        // Instantiate corner GameObjects or assign them if they are already in the scene
        topLeftCorner = new GameObject("TopLeftCorner");
        topRightCorner = new GameObject("TopRightCorner");
        bottomLeftCorner = new GameObject("BottomLeftCorner");
        bottomRightCorner = new GameObject("BottomRightCorner");

        // Initially set the corner positions based on the collider's current bounds
        SetCornerPositions();
    }

    private void SetCornerPositions()
    {
        Physics2D.SyncTransforms();

        Vector3 topRight = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.max.y, 0);
        Vector3 topLeft = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.max.y, 0);
        Vector3 bomottRight = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, 0);
        Vector3 bottomLeft = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.min.y, 0);

        topLeftCorner.transform.parent = this.transform;
        topRightCorner.transform.parent = this.transform;
        bottomLeftCorner.transform.parent = this.transform;
        bottomRightCorner.transform.parent = this.transform;
    }

    private void UpdateCornerPositions()
    {
        // Recalculates the positions of the corner GameObjects after the collider has been resized or moved
        Vector2 offset = boxCollider.offset;
        Vector2 size = boxCollider.size;
        Vector3 centerPosition = transform.position + new Vector3(offset.x, offset.y, 0);

        topLeftCorner.transform.position = centerPosition + new Vector3(-size.x / 2, size.y / 2, 0);
        topRightCorner.transform.position = centerPosition + new Vector3(size.x / 2, size.y / 2, 0);
        bottomLeftCorner.transform.position = centerPosition + new Vector3(-size.x / 2, -size.y / 2, 0);
        bottomRightCorner.transform.position = centerPosition + new Vector3(size.x / 2, -size.y / 2, 0);
    }

    IEnumerator AdjustColliderRoutine()
    {
        // Coroutine to periodically check and adjust the size and position of the collider
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
        // Coroutine to handle the expansion of the collider
        while (detectedCollidersTree.Count < requiredTreeCount && !isSafeZoneDetected)
        {
            boxCollider.size += new Vector2(expansionStep, 0);
            UpdateCornerPositions(); // Update the corner positions

            if (!IsWallOnRightSide())
            {
                boxCollider.offset += new Vector2(expansionStep / 2, 0);
                UpdateCornerPositions(); // Update the corner positions
            }

            yield return new WaitForFixedUpdate();

            if (IsWallOnRightSide())
            {
                AdjustColliderToWall();
                break;
            }
        }
    }

    private bool ShouldExpand()
    {
        // Determines if the collider should expand based on the number of detected tree colliders and safe zone status
        return detectedCollidersTree.Count < requiredTreeCount && !isSafeZoneDetected;
    }

    private void AdjustForNewObstacles()
    {
        // Adjusts the collider in response to newly detected walls
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
        // Returns the rightmost wall detected by sorting walls based on their x position
        return detectedCollidersWall.OrderByDescending(wall => wall.transform.position.x).FirstOrDefault();
    }

    private bool IsWallOnRightSide()
    {
        // Check if any detected walls are located to the right of the collider's current right boundary
        return detectedCollidersWall.Any(wall => wall.transform.position.x > boxCollider.bounds.max.x);
    }

    private void AdjustColliderToWall()
    {
        // Adjust the width of the collider so it does not extend beyond a detected wall on the right
        Collider2D rightmostWallInsideBounds = detectedCollidersWall.OrderByDescending(wall => wall.transform.position.x).FirstOrDefault();
        if (rightmostWallInsideBounds != null)
        {
            float wallPositionX = rightmostWallInsideBounds.transform.position.x;
            float colliderRightBoundary = boxCollider.bounds.max.x;
            if (wallPositionX < colliderRightBoundary)
            {
                float newWidth = wallPositionX - boxCollider.bounds.min.x;
                boxCollider.size = new Vector2(newWidth, boxCollider.size.y);
                UpdateCornerPositions(); // Update the corner positions
            }
        }
    }

    private void AdjustCollider(Collider2D rightmostWall)
    {
        // Adjusts the width of the collider to match the position of the rightmost detected wall
        Physics2D.SyncTransforms();
        Physics2D.SyncTransforms();
        float wallPositionX = rightmostWall.transform.position.x;
        float colliderLeftBoundary = transform.position.x + boxCollider.offset.x - boxCollider.size.x / 2;

        if (wallPositionX > colliderLeftBoundary)
        {
            float newWidth = wallPositionX - colliderLeftBoundary;
            boxCollider.size = new Vector2(newWidth, boxCollider.size.y);
            UpdateCornerPositions(); // Update the corner positions
            Physics2D.SyncTransforms();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Trigger detection for trees, walls, and safe zones
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
        // Handle objects exiting the trigger zone
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
