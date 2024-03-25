using UnityEngine;
using System.Collections.Generic;

public class DetectionZoneForPlayer : MonoBehaviour
{
    public List<UpgradeManager> detectedUpgradeManagers = new List<UpgradeManager>();
    public List <barracksUI> detectedbarracksUI = new List<barracksUI>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Upgradable")) // Using a more appropriate tag
        {
            var upgradeManager = collision.GetComponent<UpgradeManager>();
            var barracksUI = collision.GetComponent<barracksUI>();
            if (upgradeManager != null && !detectedUpgradeManagers.Contains(upgradeManager))
            {
                detectedUpgradeManagers.Add(upgradeManager);
            }
            else if (barracksUI != null && !detectedbarracksUI.Contains(barracksUI))
            {
                detectedbarracksUI.Add(barracksUI);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Upgradable")) // Ensuring tag consistency
        {
            var upgradeManager = collision.GetComponent<UpgradeManager>();
            var barracksUI = collision.GetComponent<barracksUI>();
            if (upgradeManager != null)
            {
                detectedUpgradeManagers.Remove(upgradeManager);
            }
            else if (barracksUI != null )
            {
                detectedbarracksUI.Remove(barracksUI);
            }
        }
    }

    public UpgradeManager FindUpgradeManager()
    {
        if (detectedUpgradeManagers.Count > 0)
        {
            return detectedUpgradeManagers[0]; // Returns the first detected UpgradeManager
        }
        return null;
    }

    public barracksUI FindBarracksUI()
    {
        if (detectedUpgradeManagers.Count > 0)
        {
            return detectedbarracksUI[0]; // Returns the first detected UpgradeManager
        }
        return null;
    }
}
