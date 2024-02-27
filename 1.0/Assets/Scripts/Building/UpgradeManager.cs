using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradeUI; // Assign in inspector, the UI that should appear

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            ShowUpgradeUI(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowUpgradeUI(false);
        }
    }

    void ShowUpgradeUI(bool show)
    {
        upgradeUI.SetActive(show);
    }
}
