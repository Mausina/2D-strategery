using System.Collections;
using UnityEngine;



public class AnimalController : MonoBehaviour
{

    private Animator animator;
    public float speed = 5f;
    private bool isMoving = true;
    private bool isMovingRight = true;
    public delegate void AnimalDeactivatedHandler(GameObject animal);
    public event AnimalDeactivatedHandler OnAnimalDeactivated;

    public void Deactivate()
    {
        // Set isAlive to false to play death animation
        animator.SetBool("isAlive", false);

        // Optional: Wait for animation to finish before deactivating
        StartCoroutine(DeactivateAfterAnimation());
    }

    IEnumerator DeactivateAfterAnimation()
    {
        // Wait for the death animation to finish. Adjust the time according to your animation's length
        yield return new WaitForSeconds(1.5f); // Assuming the death animation is about 1.5 seconds long
        Debug.Log("DeactivateAfterAnimation complete", gameObject);
        OnAnimalDeactivated?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", isMoving);
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
