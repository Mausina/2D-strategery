using UnityEngine;
using UnityEngine.UI; // Make sure you have this using directive if you're interacting with UI elements

public class UpgradeUIManager : MonoBehaviour
{
    /*
    public static UpgradeUIManager Instance { get; private set; }
    public GameObject upgradeUIButton; // Assign in Inspector

    private Wall selectedWall; // Variable to keep track of the currently selected wall

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance alive across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowUpgradeOptions(Wall wall)
    {
        selectedWall = wall; // Store the reference to the wall that is being upgraded
        upgradeUIButton.SetActive(true);
        // Position UI near wall or set it to follow the wall on screen
    }

    public void HideUpgradeOptions()
    {
        upgradeUIButton.SetActive(false);
    }

    // Call this method from your upgrade button's OnClick event
    public void UpgradeWall(Wall wall)
    {
        // Assuming each WallLevel has an 'upgradeCost' property.
        int upgradeCost = wall.wallLevels[wall.level].upgradeCost;
        if (CoinManager.Instance.coins >= upgradeCost)
        {
            CoinManager.Instance.SubtractCoins(upgradeCost);
            wall.UpgradeWall(); // Assuming this method upgrades the wall and handles visual/logic updates.
        }
        else
        {
            Debug.Log("Not enough coins.");
            // Optionally, trigger some feedback to the player here.
        }
        HideUpgradeOptions();
    }
    */
}

