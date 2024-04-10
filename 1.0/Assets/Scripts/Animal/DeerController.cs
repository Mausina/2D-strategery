using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerController : MonoBehaviour
{
    private Animator animator;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    private bool isMoving = true;
    private bool isMovingRight = true;
    private bool isRunning = false; // Track running state

    public delegate void AnimalDeactivatedHandler(GameObject animal);
    public event AnimalDeactivatedHandler OnAnimalDeactivated;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
        // Initially, the animal is walking
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning); // Add this line
        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        AdjustFacingDirection();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Decide immediately to run forward or backward with a 50% chance
            if (Random.Range(0, 100) < 50)
            {
                isMovingRight = !isMovingRight; // Change direction
            }
            isRunning = true; // Start running
            animator.SetBool("isRunning", isRunning); // Notify the animator
            StartCoroutine(RunForAWhile()); // Run for a random duration
        }
    }

    IEnumerator RunForAWhile()
    {
        float runDuration = Random.Range(2f, 4f);
        yield return new WaitForSeconds(runDuration);
        isRunning = false; // Stop running after the duration
        animator.SetBool("isRunning", isRunning); // Notify the animator
    }

    IEnumerator TurnAroundRoutine(bool forceTurn)
    {
        StopMoving(); // Ensure movement stops immediately

        float stopTime = Random.Range(3f, 5f);
        yield return new WaitForSeconds(stopTime);

        if (forceTurn || Random.Range(0, 100) < 40) // 40% chance to turn around
        {
            isMovingRight = !isMovingRight;
        }

        ResumeMoving(); // Resume movement only after the pause
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (isMoving)
            {
                float moveTime = isRunning ? Random.Range(2f, 4f) : Random.Range(4f, 7f);
                float endTime = Time.time + moveTime;

                while (Time.time < endTime && isMoving)
                {
                    Move(isRunning ? runSpeed : walkSpeed);
                    yield return null;
                }
            }

            yield return StartCoroutine(TurnAroundRoutine(false));
        }
    }

    void Move(float speed)
    {
        if (!isMoving) return;

        float step = speed * Time.deltaTime;
        transform.position += new Vector3(isMovingRight ? step : -step, 0, 0);
    }

    void StopMoving()
    {
        isMoving = false;
        animator.SetBool("isMoving", isMoving);
    }

    void ResumeMoving()
    {
        isMoving = true;
        animator.SetBool("isMoving", isMoving);
    }

    void AdjustFacingDirection()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = isMovingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    public void Deactivate()
    {
        animator.SetBool("isAlive", false);
        StartCoroutine(DeactivateAfterAnimation());
    }

    IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("DeactivateAfterAnimation complete", gameObject);
        OnAnimalDeactivated?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}
