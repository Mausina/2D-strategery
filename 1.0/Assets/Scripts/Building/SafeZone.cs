using UnityEngine;

public class SafeZone : MonoBehaviour
{
    BoxCollider2D safezone;

    private void Awake()
    {
        safezone = GetComponent<BoxCollider2D>();
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
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
                float expansion = wallRightEdge - safezoneRight - 5f;
                Vector2 newSize = safezone.size;
                newSize.x += expansion; // Expand the safezone's width

                // Adjust the position so the expansion only happens to the right
                Vector2 newPosition = safezone.offset;
                newPosition.x += expansion / 2f;

                safezone.size = newSize;
                safezone.offset = newPosition;
            }
        }
    }

}
