using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private Collider2D col;
    public Wall wall; // Set this reference in the Unity Inspector

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
        float objectHeight = CalculateObjectHeight(collision);
        Debug.Log($"Object Entered: {collision.name}, Height: {objectHeight}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
        float objectHeight = CalculateObjectHeight(collision);
        Debug.Log($"Object Exited: {collision.name}, Height: {objectHeight}");
    }
    public void CheckForBuilder()
    {
        foreach (var collider in detectedColliders)
        {
            if (collider.CompareTag("Builder")) // Assuming your builders have a tag "Builder"
            {
                // Now call the instance method on the wall instance
                if (wall != null) // Check to ensure wall is not null
                {
                   // wall.StartUpgradeProcess(); // This should now be an instance method call
                }
                else
                {
                    Debug.LogError("Wall reference not set in the DetectionZone.");
                }
                return;
            }
        }
    }

    public float CalculateObjectHeight(Collider2D collider)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            return boxCollider.size.y * collider.transform.localScale.y;
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            return 2 * circleCollider.radius * collider.transform.localScale.y;
        }
        else if (collider is PolygonCollider2D polyCollider)
        {
            return polyCollider.bounds.size.y;
        }
        else if (collider is CapsuleCollider2D capsuleCollider)
        {
            return capsuleCollider.size.y * collider.transform.localScale.y;
        }
        else
        {
            Debug.LogWarning("Unsupported collider type for height calculation.");
            return 0;
        }
    }
}
