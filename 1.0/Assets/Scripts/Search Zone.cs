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
        if (detectedCollidersWall.Count == 0 && !isSafeZoneDetected)
        {
            while (detectedCollidersTree.Count < requiredTreeCount && !isSafeZoneDetected)
            {
                boxCollider.size += new Vector2(expansionStep, 0);
                boxCollider.offset += new Vector2(expansionStep / 2, 0);
                yield return new WaitForFixedUpdate();
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
