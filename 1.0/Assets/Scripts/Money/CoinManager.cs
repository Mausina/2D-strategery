using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coins = 0;
    public Text coinText; // Ensure this is assigned in the Inspector to your UI Text element

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive when loading new scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinText(); // Update the UI on start
    }

    public void AddCoins(int amount)
    {
        coins += amount;
       // Debug.Log("Coins: " + coins);
        UpdateCoinText(); // Update the UI whenever coins are added
    }

    public void SubtractCoins(int amount)
    {
        coins -= amount;
        //Debug.Log(coins);
        if (coins < 0) coins = 0;
        UpdateCoinText(); // Update the UI whenever coins are subtracted
    }

    // Check if the player can afford a cost
    public bool CanAfford(int cost)
    {
        return coins >= cost;
    }

    // Spend coins if the player can afford it
    public void SpendCoins(int amount)
    {
        if (CanAfford(amount))
        {
            SubtractCoins(amount); // Use the SubtractCoins method to ensure the UI is updated
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    // Update the UI text to show the current coin count
    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins.ToString();
        }
    }
}

