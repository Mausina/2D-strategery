using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportDestination;
    public float cooldown = 2f; // Cooldown in seconds before the teleporter can be used again
    private bool isCooldown = false; // Internal cooldown state

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Teleport the object that entered the trigger
        TeleportObject(other.transform);

        // Optionally, teleport all attached/associated objects
        // This could be adapted based on how you define "attached" (e.g., via a list, tag, etc.)
        foreach (Transform child in other.transform)
        {
            TeleportObject(child);
        }
        TeleportCooldown();
    }

    private void TeleportObject(Transform objectToTeleport)
    {
        objectToTeleport.position = teleportDestination.position;
    }


    private IEnumerator TeleportCooldown()
    {
        // Activate cooldown
        isCooldown = true;
        // Wait for the specified cooldown duration
        yield return new WaitForSeconds(cooldown);
        // Deactivate cooldown
        isCooldown = false;
    }
}


