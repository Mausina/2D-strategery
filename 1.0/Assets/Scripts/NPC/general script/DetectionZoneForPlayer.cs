using UnityEngine;
using System.Collections.Generic;

public class DetectionZoneForPlayer : MonoBehaviour
{
    public List<UpgradeManager> detectedUpgradeManagers = new List<UpgradeManager>();
    public List<barracksUI> detectedbarracksUI = new List<barracksUI>();
    public List<IMountable> detectedMountables = new List<IMountable>();

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
        else if (collision.CompareTag("Mountable")) // Assuming you have a tag for mountable animals
        {
            AddDetectedComponent<IMountable>(collision, detectedMountables);
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
            else if (barracksUI != null)
            {
                detectedbarracksUI.Remove(barracksUI);
            }
        }
        else if (collision.CompareTag("Mountable")) // Make sure to be consistent with the tags used
        {
            RemoveDetectedComponent<IMountable>(collision, detectedMountables);
        }
    }

    private void AddDetectedComponent<T>(Collider2D collision, List<T> list) where T : class
    {
        var component = collision.GetComponent<T>();
        if (component != null && !list.Contains(component))
        {
            list.Add(component);
        }
    }

    private void RemoveDetectedComponent<T>(Collider2D collision, List<T> list) where T : class
    {
        var component = collision.GetComponent<T>();
        if (component != null)
        {
            list.Remove(component);
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
        if (detectedbarracksUI.Count > 0)
        {
            return detectedbarracksUI[0]; // Returns the first detected barracksUI
        }
        return null;
    }

    public IMountable FindMountable()
    {
        if (detectedMountables.Count > 0)
        {
            return detectedMountables[0]; // Returns the first detected IMountable
        }
        return null;
    }
}
