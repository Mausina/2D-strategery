using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrade UI")]
    public GameObject coinIcon; // Assign your 3D model or sprite in the inspector
    public GameObject coinPrefab;
    public int upgradeCost = 1;
    public Wall wall; // Assign this in the inspector

    private bool playerInRange = false;
    private int coinsInserted = 0;

    private void Awake()
    {
        playerInRange = false;
        coinIcon.SetActive(false); // Start with the coin icon hidden

    }


    private void Update()
    {
        if (playerInRange)
        {
            if (!coinIcon.activeSelf)
            {
                coinIcon.SetActive(true);
                coinsInserted = 0;
            }

            if (Input.GetKeyDown(KeyCode.U) && CoinManager.Instance.coins > 0)
            {
                InsertCoin();
            }
        }
        else
        {
            if (coinIcon.activeSelf)
            {
                coinIcon.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }

    private bool coinInserted = false; // Flag to prevent multiple coins from being inserted

    private void InsertCoin()
    {
        Debug.Log("Attempting to insert coin.");
        if (CoinManager.Instance.coins > 0 && !coinInserted)
        {
            Debug.Log("Coin inserted.");
            coinInserted = true; // Prevent further coin insertion until this one is processed
            CoinManager.Instance.SubtractCoins(1); // Subtract one coin

            GameObject coin = Instantiate(coinPrefab, coinIcon.transform.position, Quaternion.identity);

            coinsInserted++;
            Debug.Log("Upgrade cost met.");
            Debug.Log(coinsInserted);
            Debug.Log(upgradeCost);
            if (coinsInserted >= upgradeCost)
            {
                // Find the Wall GameObject by tag and get the Wall component
                GameObject wallObject = GameObject.FindGameObjectWithTag("Wall");
                if (wallObject != null)
                {
                    Wall wall = wallObject.GetComponent<Wall>();
                    if (wall != null)
                    {
                        wall.UpgradeWall();
                    }
                    else
                    {
                        Debug.Log("Wall component not found on tagged object.");
                    }
                }
                else
                {
                    Debug.Log("Wall GameObject not found by tag.");
                }

                coinsInserted = 0; // Reset for the next upgrade
            }


            Destroy(coin, 1.0f); // Destroy the coin after a delay, adjust as needed
            coinInserted = false; // Allow new coins to be inserted
        }
    }
}



