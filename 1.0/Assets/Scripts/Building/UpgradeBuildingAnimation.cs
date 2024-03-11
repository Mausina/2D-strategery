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
        // Запуск или остановка анимации на 1 секунду
        animator.SetBool("isUpgrading", isUpgrading);
        if (isUpgrading)
        {
            // Если необходимо, можно добавить задержку или другую логику здесь
        }
    }

    public void CompleteUpgrade()
    {
        animator.SetBool("isUpgrading", false);
        animator.SetBool("isCompleted", true);
        OnUpgradeComplete?.Invoke();
    }
}
