using System;
using System.Collections;
using UnityEngine;

namespace Archer
{
    public class ArcherController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float runSpeed = 4f;
        archerShooting archerShooting;
        private bool isPaused = false;
        private GameObject searchZone;
        private GameObject safeZone;
        private bool isMovingRight = true;
        WorldPoolManager poolManager;
        private Vector3 bottomLeftCorner;
        private Vector3 bottomRightCorner;
        void Start()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(BehaviorController());
            isMovingRight = true;
        }
        private void Awake()
        {
            archerShooting = GetComponent<archerShooting>();
            OnDrawGizmos();
        }

        private void Update()
        {
            if (archerShooting != null)
            {
                archerShooting.CheckDetectionZone();
            }
            UpdatePatrolPoints();
        }
        private void UpdatePatrolPoints()
        {
            // Assuming 'BottomLeftCorner' and 'BottomRightCorner' are transform names of children of searchZone.
            if (searchZone != null)
            {
                bottomLeftCorner = searchZone.transform.Find("BottomLeftCorner").position;
                bottomRightCorner = searchZone.transform.Find("BottomRightCorner").position;
            }
        }
        public void AssignPool(WorldPoolManager newPool)
        {
          poolManager = newPool;
        }

        public void SetSafeZone(GameObject zone)
        {
            safeZone = zone;
        }
        private void SetPatrolPoints()
        {
            if (searchZone != null)
            {
                Transform blcTransform = searchZone.transform.Find("BottomLeftCorner");
                Transform brcTransform = searchZone.transform.Find("BottomRightCorner");

                if (blcTransform != null && brcTransform != null)
                {
                    bottomLeftCorner = blcTransform.position;
                    bottomRightCorner = brcTransform.position;
                    Debug.Log("Bottom Right Corner Position: " + bottomRightCorner + ", Bottom Left Corner Position: " + bottomLeftCorner);
                }
                else
                {
                    Debug.LogError("Patrol points not found. Check names and hierarchy.");
                }
            }
            else
            {
                Debug.LogError("Search zone not set.");
            }
        }

        void OnDrawGizmos()
        {
            if (searchZone != null)
            {
                // Set the color of the Gizmos
                Gizmos.color = Color.red;

                // Draw small spheres at the patrol points
                Gizmos.DrawSphere(bottomLeftCorner, 1f);
                Gizmos.DrawSphere(bottomRightCorner, 1f);

                // Draw a line connecting the patrol points
                Gizmos.DrawLine(bottomLeftCorner, bottomRightCorner);
            }
        }

        public void SetSearchZone(GameObject zone)
        {
            searchZone = zone;
            Debug.Log("SetSearchZone: " + zone);
            SetPatrolPoints();
        }

        private IEnumerator BehaviorController()
        {
            while (true)
            {
                if (IsNightTime())
                {
                    yield return StartCoroutine(RunToSafeZone());
                }
                else
                {
                    yield return StartCoroutine(Patrol());
                }
            }
        }
        public void PauseMovement()
        {
            isPaused = true;
            //animator.SetBool("isMove", false); // Assuming there is an 'isMove' animation parameter
        }

        public void ResumeMovement()
        {
            isPaused = false;
        }
        private IEnumerator Patrol()
        {
            isMovingRight = true; // Assuming starting direction.
            FlipDirection(isMovingRight); // Ensure the archer faces the right direction initially.

            while (true)
            {
                if (!isPaused)
                {
                    Vector3 target = isMovingRight ? searchZone.transform.Find("BottomRightCorner").position
                                                   : searchZone.transform.Find("BottomLeftCorner").position;

                    float patrolTime = UnityEngine.Random.Range(5f, 8f);
                    float timePassed = 0f;

                    // Patrol towards the target with checks to ensure responsiveness to pause.
                    while (Vector3.Distance(transform.position, target) > 1.5f && !isPaused)
                    {
                        Move(target, patrolSpeed);
                        timePassed += Time.deltaTime;

                        if (timePassed >= patrolTime)
                        {
                            float actionChance = UnityEngine.Random.value;
                            HandlePatrolDecision(actionChance);

                            patrolTime = UnityEngine.Random.Range(5f, 8f);
                            timePassed = 0f;  // Reset timer
                        }

                        target = UpdateTarget(); // Always get the most current target
                        yield return null;
                    }
                    if (!isPaused) FlipDirection(isMovingRight = !isMovingRight);
                }
                yield return null;
            }
        }

        private void HandlePatrolDecision(float chance)
        {
            if (chance < 0.5f)
            {
                // 50% chance to pause. Implement wait directly here to ensure it handles immediately.
                StartCoroutine(PauseTemporarily(UnityEngine.Random.Range(5f, 8f)));
            }
            else if (chance < 0.9f)
            {
                // 40% chance to turn around immediately
                FlipDirection(isMovingRight = !isMovingRight);
            }
        }

        private IEnumerator PauseTemporarily(float duration)
        {
            PauseMovement();
            yield return new WaitForSeconds(duration);
            ResumeMovement();
        }

        private Vector3 UpdateTarget()
        {
            return isMovingRight ? searchZone.transform.Find("BottomRightCorner").position
                                 : searchZone.transform.Find("BottomLeftCorner").position;
        }


        private void FlipDirection(bool facingRight)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = facingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }






        private IEnumerator RunToSafeZone()
        {
            yield return null;
        }

        private void Move(Vector3 target, float speed)
        {
            target.y = transform.position.y; // Keeps the movement in the horizontal plane
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            Debug.Log("Moving to target: " + target + " Step: " + step);
        }


        private bool IsNightTime()
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            return currentTime.Hours < 6 || currentTime.Hours >= 18;
        }


    }
}

