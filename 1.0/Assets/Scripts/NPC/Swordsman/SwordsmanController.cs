using System.Collections;
using UnityEngine;

public class SwordsmanController : MonoBehaviour
{
    public Transform[] targets; // Assign in the inspector
    private Transform target;
    private Animator animator;
    public float speed = 5f;
    private bool isMoving = false;
    private bool isMovingRight = true;
    private bool hasReachedTarget = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        ChooseTarget(initial: true); // Ensure the initial target is chosen
        StartCoroutine(MoveAndStopRoutine());
    }

    void Update()
    {
        AdjustFacingDirection();
    }

    void ChooseTarget(bool initial = false)
    {
        if (!initial)
        {
            // After reaching a target, decide whether to move forward or turn around
            if (Random.Range(0, 100) < 60)
            {
                // 60% chance to move forward
                Transform newTarget;
                do
                {
                    newTarget = targets[Random.Range(0, targets.Length)];
                } while (targets.Length > 1 && newTarget == target);
                target = newTarget;
            }
            else
            {
                // 40% chance to turn around
                isMovingRight = !isMovingRight; // Simply reverse the direction flag
                // Choose a target in the new direction if applicable
                // This logic assumes you might have targets suitable for turning around.
                // If your targets are not positioned for this, you might need a different approach.
            }
        }
        else
        {
            // Initial target selection
            target = targets[Random.Range(0, targets.Length)];
        }
        hasReachedTarget = false;
    }

    IEnumerator MoveAndStopRoutine()
    {
        while (true)
        {
            float moveTime = Random.Range(4f, 7f);
            float startTime = Time.time;
            while (!hasReachedTarget && Time.time - startTime < moveTime)
            {
                MoveTowardsTarget();
                yield return null; // Wait for the next frame
            }

            MoveTowardsTarget();
            yield return null; // Wait for the next frame

            // NPC has reached the target, stop moving
            isMoving = false;
            animator.SetBool("isMoving", isMoving);
            float stopTime = Random.Range(4f, 8f);
            yield return new WaitForSeconds(stopTime);

            // After stopping, choose a new target to simulate walking near objects randomly
            ChooseTarget();
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null && !hasReachedTarget)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > 0.1f)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
                isMoving = true;
                animator.SetBool("isMoving", isMoving);

                // Check if we need to update the facing direction
                isMovingRight = (target.position.x - transform.position.x) > 0;
            }
            else
            {
                // The target has been reached
                hasReachedTarget = true;
            }
        }
    }

    private void AdjustFacingDirection()
    {
        // Adjust the facing direction of the NPC based on isMovingRight
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}
