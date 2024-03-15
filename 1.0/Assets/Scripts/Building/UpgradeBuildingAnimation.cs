using System;
using UnityEngine;

public class UpgradeBuildingAnimatio : MonoBehaviour
{
    public event Action OnUpgradeComplete;
    public Animator animator;

    public bool IsUpgrading1 { get; private set; }= false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void SetAnimatorSpeed(float speedMultiplier)
    {
        //Debug.Log(speedMultiplier);
        animator.speed = speedMultiplier; // Adjust the animator's speed.
    }

    public void SetUpgradeAnimationState(bool isUpgrading)
    {
        
        Debug.Log(isUpgrading);
        animator.SetBool("isUpgrading", isUpgrading); // Set the animation state.

    }

    public void CompleteUpgrade()
    {
        IsUpgrading1 = false;
        SetUpgradeAnimationState(false); // Stop the upgrading animation.
        animator.SetBool("isCompleted", true); // Indicate the completion of the upgrade.
        OnUpgradeComplete?.Invoke(); // Notify listeners about the completion.
    }
}
