using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class NewBehaviourScripts : MonoBehaviour
{

    public float speed;
    public Transform[] patrolPoints;
    public float waitTime;
    public float walkStopRate = 0.6f;
    int currentPointIndex;
    bool isMoving = true;
    public float stoppingDistance = 0.1f;
    public DetectionZone attacZone;
    Animator animator;
    Damageable damageable;
    [SerializeField] private Rigidbody2D rb;

    public bool _hasTarget = false;

    public bool HesTarget { get {return _hasTarget;} private set { _hasTarget = value; animator.SetBool(AnimationStrings.hasTarget, value);} }

    void Update()
    {
        if (CanMove)
        {
            float distanceToTarget = Vector3.Distance(transform.position, patrolPoints[currentPointIndex].position);

            // Если расстояние больше stoppingDistance, двигаемся к цели
            if (distanceToTarget > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, speed * Time.deltaTime);
            }
            else
            {
                // Если враг достиг целевой точки, переключаемся на следующую точку
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                Flip();
            }

            HesTarget = attacZone.detectedColliders.Count > 0;
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }


    }
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.LockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.LockVelocity, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    void MoveToNextPatrolPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, speed * Time.deltaTime);

        if (transform.position == patrolPoints[currentPointIndex].position)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; // Move to the next point or loop back to the start
            StartCoroutine(Wait());
        }

    }

    IEnumerator Wait()
    {
        isMoving = false;
        yield return new WaitForSeconds(waitTime);
        isMoving = true;
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;  
        transform.localScale = newScale;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        
        // Disable the lockVelocity variable for 1 second
        LockVelocity = false;
        StartCoroutine(EnableLockVelocityAfterDelay(1.0f));

        // Apply the knockback to the enemy's velocity
        rb.velocity += knockback;
    }

    private IEnumerator EnableLockVelocityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LockVelocity = true;
    }

}

