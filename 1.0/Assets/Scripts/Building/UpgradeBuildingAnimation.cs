using System;
using System.Collections;
using UnityEngine;

public class UpgradeBuildingAnimatio : MonoBehaviour
{
    private Animator animator;
    public event Action OnUpgradeComplete; // Delegate for upgrade completion notification.
    private Coroutine upgradeCoroutine; // Reference to the currently running upgrade coroutine.
    private bool isBuilderPresent = false; // Tracks the presence of the builder.

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Called to start or stop the upgrade animation based on the builder's presence and a specified duration.
    public void StartUpgradeAnimation(bool start, float duration)
    {
        if (start)
        {
            if (upgradeCoroutine != null)
            {
                StopCoroutine(upgradeCoroutine); // Ensure that only one upgrade process runs at a time.
            }
            upgradeCoroutine = StartCoroutine(CompleteUpgradeAfterDelay(duration));
            animator.SetBool("isUpgrading", true);
            animator.speed = 1; // Ensure animation plays at normal speed.
        }
        else
        {
            if (upgradeCoroutine != null)
            {
                StopCoroutine(upgradeCoroutine); // Stop the coroutine if the builder leaves.
                upgradeCoroutine = null;
            }
            animator.SetBool("isUpgrading", false);
            animator.speed = 0; // Pause the animation.
        }
    }

    // Coroutine to wait for the specified duration before marking the upgrade as complete.
    private IEnumerator CompleteUpgradeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CompleteUpgrade(true);
    }

    // Called to finalize the upgrade, potentially by the coroutine or directly if needed.
    public void CompleteUpgrade(bool finish)
    {
        if (finish)
        {
            animator.SetBool("isCompleted", finish);
            animator.speed = 1; // Resume normal animation speed for subsequent animations.
            OnUpgradeComplete?.Invoke(); // Notify listeners of upgrade completion.
        }
    }

    // Optionally called by the builder to indicate its arrival or departure.
    public void SetBuilderPresence(bool presence)
    {
        isBuilderPresent = presence;
        if (!isBuilderPresent && upgradeCoroutine != null)
        {
            StopCoroutine(upgradeCoroutine); // Optionally stop the upgrade if the builder leaves.
            animator.SetBool("isUpgrading", false);
            animator.speed = 0; // Optionally freeze animation to indicate interruption.
            upgradeCoroutine = null;
        }
        else if (isBuilderPresent && upgradeCoroutine == null)
        {
            // If needed, restart the upgrade process when the builder returns.
            // This would require knowing the intended duration and possibly adjusting logic to restart.
        }
    }
}
