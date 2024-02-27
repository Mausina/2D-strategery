using UnityEngine;

public class WallUpgradeManager : MonoBehaviour
{
    /*
    public GameObject upgradeIconPrefab; // Assign your upgrade icon prefab here
    public Transform iconSpawnPoint; // Assign a point in your scene where icons should appear
    public Wall wall; // Reference to the Wall script component
    private GameObject[] upgradeIcons; // To keep track of instantiated icons

    private void Start()
    {
        if (wall == null)
        {
            Debug.LogError("WallUpgradeManager: No reference to Wall script set.");
            return;
        }
        else
        {
            Debug.Log("WallUpgradeManager: Wall reference set correctly.");
        }

        upgradeIcons = new GameObject[wall.maxLevel]; // Initialize the array based on the max level of the wall
        Debug.Log("WallUpgradeManager: Upgrade icons array initialized with size " + wall.maxLevel);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            Debug.Log("WallUpgradeManager: Player entered trigger zone.");
            ShowUpgradeIcons();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("WallUpgradeManager: Player exited trigger zone.");
            HideUpgradeIcons();
        }
    }

    private void ShowUpgradeIcons()
    {
        for (int i = 0; i < wall.level; i++) // Loop through based on the current level
        {
            if (upgradeIcons[i] == null) // If the icon doesn't exist, instantiate it
            {
                Debug.Log($"WallUpgradeManager: Instantiating upgrade icon for level {i + 1}.");
                upgradeIcons[i] = Instantiate(upgradeIconPrefab, iconSpawnPoint.position, Quaternion.identity);
                upgradeIcons[i].transform.SetParent(iconSpawnPoint, false); // Set the icon's parent to the spawn point
                // Assign the upgrade logic to the icon, possibly passing 'i' as the upgrade level it represents
                // upgradeIcons[i].GetComponent<YourUpgradeButtonScript>().Setup(i, wall);
            }
            else
            {
                Debug.Log($"WallUpgradeManager: Upgrade icon for level {i + 1} already exists.");
            }
            upgradeIcons[i].SetActive(true); // Make the icon visible
        }
    }

    private void HideUpgradeIcons()
    {
        foreach (var icon in upgradeIcons)
        {
            if (icon != null)
            {
                Debug.Log("WallUpgradeManager: Hiding upgrade icon.");
                icon.SetActive(false); // Hide the icon
            }
        }
    }
    */
}
