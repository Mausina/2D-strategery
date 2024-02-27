using UnityEngine;
using System.Collections;

public class CoinToUpgradeSlot : MonoBehaviour
{
    public GameObject upgradeSlot; // Assign the upgrade slot in the inspector
    public int value = 1;

    private GameObject player;
    private bool isBeingUsedForUpgrade = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        upgradeSlot = GameObject.FindGameObjectWithTag("UpgradeSlot");
    }

    public void UseCoinForUpgrade()
    {
        if (!isBeingUsedForUpgrade)
        {
            StartCoroutine(MoveCoinToUpgradeSlot());
        }
    }

    private IEnumerator MoveCoinToUpgradeSlot()
    {
        isBeingUsedForUpgrade = true; // The coin is being used, so set the flag

        // Optional: Add a sound effect or particle effect here

        float duration = 0.5f; // The duration of the movement towards the upgrade slot
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = upgradeSlot.transform.position; // The position to move the coin to

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Here you would typically notify the upgrade manager that a coin has been inserted
        // For example:
        // upgradeManager.ReceiveCoin(value);

        Destroy(gameObject); // Destroy the coin object after it moves to the slot
    }
}
