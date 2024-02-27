using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coins = 0;
    public Text coinText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       // UpdateCoinText();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("sum:" + coins);
       // UpdateCoinText();
    }

    public void SubtractCoins(int amount)
    {
        coins -= amount;
        if (coins < 0) coins = 0;
       // UpdateCoinText();
    }

    /*
    void UpdateCoinText()
    {
        if (coinText != null)
        {
           // coinText.text = "Coins: " + coins.ToString();
        }
    }
    */
}
