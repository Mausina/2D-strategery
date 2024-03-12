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

    public void SetUpgradeAnimationState(bool isUpgrading)
    {
        
        // «апуск или остановка анимации на 1 секунду
        animator.SetBool("isUpgrading", isUpgrading);
        if (isUpgrading = false)
        {
            animator.speed = 0;
        }

    }

    public void CompleteUpgrade()
    {
        animator.SetBool("isUpgrading", false);
        animator.SetBool("isCompleted", true);
        OnUpgradeComplete?.Invoke();
    }
}
