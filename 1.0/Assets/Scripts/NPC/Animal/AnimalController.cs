using System.Collections;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private Animator animator;
    public float speed = 5f;
    private bool isMoving = true;
    private bool isMovingRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", isMoving);
        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        AdjustFacingDirection();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree") || collision.CompareTag("Wall"))
        {
            StartCoroutine(TurnAroundRoutine(true));
        }
    }

    IEnumerator TurnAroundRoutine(bool forceTurn)
    {
        StopMoving(); // Ensure movement stops immediately

        float stopTime = Random.Range(3f, 5f);
        yield return new WaitForSeconds(stopTime);

        if (forceTurn || Random.Range(0, 100) < 40) // Condition for turning around
        {
            isMovingRight = !isMovingRight;
        }

        ResumeMoving(); // Resume movement only after the pause
    }
    IEnumerator MoveRoutine()
    {
        while (true)
        {
            // Only initiate movement if isMoving is true
            if (isMoving)
            {
                float moveTime = Random.Range(4f, 7f);
                float endTime = Time.time + moveTime;

                while (Time.time < endTime && isMoving) // Also check isMoving here to ensure immediate response
                {
                    Move();
                    yield return null;
                }
            }

            // After moving or when deciding to stop, pause movement then decide next action
            yield return StartCoroutine(TurnAroundRoutine(false));
        }
    }

    void Move()
    {
        if (!isMoving) return; // Exit the method if isMoving is false

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
}
