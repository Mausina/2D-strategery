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
        // ������ ��� ��������� �������� �� 1 �������
        animator.SetBool("isUpgrading", isUpgrading);
        if (isUpgrading)
        {
            // ���� ����������, ����� �������� �������� ��� ������ ������ �����
        }
    }

    public void CompleteUpgrade()
    {
        animator.SetBool("isUpgrading", false);
        animator.SetBool("isCompleted", true);
        OnUpgradeComplete?.Invoke();
    }
}
