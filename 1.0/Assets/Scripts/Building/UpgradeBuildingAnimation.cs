using System;
using System.Collections;
using UnityEngine;

public class UpgradeBuildingAnimatio : MonoBehaviour
{
    public event Action OnUpgradeComplete;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimatorSpeed(float speedMultiplier)
    {
        Debug.Log(speedMultiplier);
        animator.speed = speedMultiplier; // Correctly adjust the animator's speed.
    }

    public void SetUpgradeAnimationState(bool isUpgrading)
    {
        animator.SetBool("isUpgrading", isUpgrading); // Simply set the animation state.
    }

    public void CompleteUpgrade()
    {
        SetUpgradeAnimationState(false); // Ensure to stop the upgrading animation.
        animator.SetBool("isCompleted", true); // This could represent the completion of the upgrade.
        OnUpgradeComplete?.Invoke(); // Notify listeners (e.g., BuilderController) about the completion.
    }
}
