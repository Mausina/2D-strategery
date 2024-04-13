using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public GameObject cornerMarker; // Assign a prefab or sprite in the inspector
    BoxCollider2D safezone;

    private void Awake()
    {
        safezone = GetComponent<BoxCollider2D>();
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        if (cornerMarker == null)
        {
            Debug.LogError("Corner Marker is not assigned!");
            this.enabled = false; // Disable script if no marker is assigned
            return;
        }

        UpdateMarkerPosition();
    }

    private void Update()
    {
        UpdateMarkerPosition(); // Update marker position in case of any changes in runtime
    }

    private void UpdateMarkerPosition()
    {
        if (safezone != null && cornerMarker != null)
        {
            // Calculate the position for the lower right corner of the collider
            Vector2 lowerRightCorner = new Vector2(
                safezone.bounds.max.x, // Right side
                safezone.bounds.min.y); // Bottom side

            // Set the corner marker to match the calculated position
            cornerMarker.transform.position = lowerRightCorner;
        }
    }

    public void ExpandSafeZone(Vector2 wallPosition, float wallWidth)
    {
        if (safezone != null)
        {
            // Get the rightmost x-coordinate of the safezone
            float safezoneRight = safezone.bounds.max.x;
            // Calculate the rightmost x-coordinate of the wall
            float wallRightEdge = wallPosition.x + wallWidth / 2f;

            // If the wall's right edge is beyond the safezone by more than 1 unit
            if (wallRightEdge - safezoneRight > 1f)
            {
                // Calculate how much to expand the safezone so it's 1 unit smaller than the wall's right edge
                float expansion = wallRightEdge - safezoneRight - 1f;
                Vector2 newSize = safezone.size;
                newSize.x += expansion; // Expand the safezone's width

                // Adjust the position so the expansion only happens to the right
                Vector2 newPosition = safezone.offset;
                newPosition.x += expansion / 2f;

                safezone.size = newSize;
                safezone.offset = newPosition;

                UpdateMarkerPosition(); // Update marker position after expanding
            }
        }
    }
}
