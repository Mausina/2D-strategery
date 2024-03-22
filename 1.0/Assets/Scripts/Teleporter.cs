using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportDestination; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = teleportDestination.position;
    }
}
