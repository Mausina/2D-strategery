using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconTriger : MonoBehaviour
{
    public GameObject upgradeIconUI; // Assign the upgrade UI in the inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure to tag your player object appropriately
        {
            Debug.Log("1");
            upgradeIconUI.SetActive(true); // Enable the upgrade UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            upgradeIconUI.SetActive(false); // Disable the upgrade UI
        }
    }
}
