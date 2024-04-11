using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMountable
{
    void Mount(PlayerController player);
    void Dismount(PlayerController player);
}

public class Wolf : MonoBehaviour, IMountable
{
    public Transform mountPoint;
    private Animator wolfAnimator; // Assign in the inspector

    void Awake()
    {
        wolfAnimator = GetComponent<Animator>();
    }

    public void Mount(PlayerController player)
    {
        // Get player's Animator and trigger the mounting animation
        Animator playerAnimator = player.GetAnimator();
        playerAnimator.SetBool("IsRiding", true);

        // Wait for the mounting animation to finish
        StartCoroutine(WaitForAnimation(playerAnimator, () =>
        {
            // Start the wolf's running animation
            wolfAnimator.SetBool("IsRunning", true);
            wolfAnimator.SetBool("IsRiding", true);
            // Disable player's movement controls
            player.DisableMovement();
            // Smoothly move the player to the mount point over time, e.g., 1 second
            StartCoroutine(MoveToMountPoint(player.transform, mountPoint.position, 1.0f));
        }));
    }
    /// <summary>
    ///  Can be chenge on animation
    /// </summary>

    IEnumerator MoveToMountPoint(Transform playerTransform, Vector3 mountPosition, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = playerTransform.position;

        while (elapsedTime < duration)
        {
            playerTransform.position = Vector3.Lerp(startingPos, mountPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is exactly at the mount point position after interpolation
        playerTransform.position = mountPosition;
        playerTransform.SetParent(mountPoint); // Parent to the mount point for following the wolf's movement
    }

    public void Dismount(PlayerController player)
    {
        // Reset animations
        Animator playerAnimator = player.GetAnimator();
        playerAnimator.SetBool("IsRiding", false);
        wolfAnimator.SetBool("IsRunning", false);

        // Handle logic for dismounting, like re-enabling player controls
    }

    IEnumerator WaitForAnimation(Animator animator, System.Action onComplete)
    {
        // Wait for the current animator state to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        onComplete?.Invoke();
    }
}
