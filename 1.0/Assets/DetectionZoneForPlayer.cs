using UnityEngine;
using System.Collections.Generic;

public class DetectionZoneForPlayer : MonoBehaviour
{
    public List<UpgradeManager> detectedUpgradeManagers = new List<UpgradeManager>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CoinUI")) // Using a more appropriate tag
        {
            var upgradeManager = collision.GetComponent<UpgradeManager>();
            if (upgradeManager != null && !detectedUpgradeManagers.Contains(upgradeManager))
            {
                detectedUpgradeManagers.Add(upgradeManager);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CoinUI")) // Ensuring tag consistency
        {
            var upgradeManager = collision.GetComponent<UpgradeManager>();
            if (upgradeManager != null)
            {
                detectedUpgradeManagers.Remove(upgradeManager);
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
}
