using System.Collections;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public Animator animator; // Ensure this is assigned, e.g., via the inspector or automatically via GetComponent<Animator>() in Start() method.
    public DetectionZone detectionZone; // Reference to the DetectionZone script, assign it in the inspector or find it automatically if it's on the same object.

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Optional: Automatically find the DetectionZone component if it's not assigned.
        if (detectionZone == null)
            detectionZone = GetComponentInChildren<DetectionZone>();
    }

    public void MoveToConstructionSite(Vector3 position, float buildTime)
    {
        StartCoroutine(BuildConstruction(position, buildTime));
    }

    IEnumerator BuildConstruction(Vector3 position, float buildTime)
    {
        // Logic to move to the position (if needed)
        // Here you can also start a "move" animation if you have one
        animator.SetBool("isMoving", true);

        // Wait until the builder reaches the construction site
        // This is a simplified approach. You may want to actually check the distance in a loop.
        yield return new WaitForSeconds(1); // Simulate the time it takes to move to the construction site

        animator.SetBool("isMoving", false);

        // Now at the construction site, start building
        animator.SetTrigger("Build");

        // Wait for build time to complete the construction
        yield return new WaitForSeconds(buildTime);

        // Optionally notify the Wall about the completion of construction
        // This could be done via an event or a direct method call if you have a reference to the Wall
    }

    private void Update()
    {
        // Use detectionZone to check if the player is within the zone and trigger animations or counters accordingly
        //if (detectionZone != null && detectionZone.IsPlayerInside)
        {
            // Player is inside the zone, you can count the time or trigger certain animations
            // This could be a good place to start an "interact" animation or increment some timer
        }
    }
}
