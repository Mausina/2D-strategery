using System;
using System.Collections;
using UnityEngine;
namespace SwordsMan
{
    public class SwordsmanController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float patrolSpeed = 3f;
        [SerializeField] public float runSpeed = 5;
        private bool isPaused = false;
        private GameObject searchZone;
        private GameObject safeZone;
        private bool isMovingRight = true;
        WorldPoolManager poolManager;
        SwordsManAttack swordsManAttack;
        private Vector3 safeZoneLeftPoint;
        private Vector3 safeZoneRightPoint;
        private Vector3 searchZoneRightPoint;

        void Start()
        {
            swordsManAttack = GetComponent<SwordsManAttack>();
            StartCoroutine(BehaviorController());
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (swordsManAttack != null) { swordsManAttack.CheckDetectionZone(); }
        }
        public void SetSafeZone(GameObject zone)
        {
            safeZone = zone;
            SetSafeZonePoints();
        }

        private IEnumerator BehaviorController()
        {
            while (true)
            {
                if (IsNightTime())
                {
                  //  yield return StartCoroutine(PatrolWait());
                    yield return StartCoroutine(RunToSafeZone());
                }
                else
                {
                    yield return StartCoroutine(PatrolWait());
                }
            }
        }

        private object StartCoroutine(IEnumerable enumerable)
        {
            throw new NotImplementedException();
        }

        private IEnumerator PatrolWait()
        {
            Vector3 targetPoint = isMovingRight ? searchZoneRightPoint : safeZoneLeftPoint;
            FlipDirection(isMovingRight);  // Ensure initial direction is correct

            float nextDecisionTime = UnityEngine.Random.Range(1f, 3f);
            float timeSinceLastDecision = 0f;
            float distanceToTravelBeforeDecision = 5f;
            Vector3 lastDecisionPoint = transform.position;

            while (!isPaused)
            {
                while (Vector3.Distance(transform.position, targetPoint) > 1f && !isPaused)
                {
                    Move(targetPoint, patrolSpeed);
                    timeSinceLastDecision += Time.deltaTime;

                    if (timeSinceLastDecision >= nextDecisionTime &&
                        Vector3.Distance(lastDecisionPoint, transform.position) >= distanceToTravelBeforeDecision)
                    {
                        yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 8f)); // Pause before making a decision
                        float actionChance = UnityEngine.Random.value;

                        if (actionChance < 0.5f)
                        {
                            nextDecisionTime = UnityEngine.Random.Range(1f, 4f);
                            FlipDirection(isMovingRight);
                        }
                        else
                        {
                            isMovingRight = !isMovingRight;
                            targetPoint = isMovingRight ? searchZoneRightPoint : safeZoneLeftPoint;
                            FlipDirection(isMovingRight);
                            nextDecisionTime = UnityEngine.Random.Range(3f, 4f);
                        }
                        timeSinceLastDecision = 0f;
                        lastDecisionPoint = transform.position;
                    }
                    yield return null;
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(4f, 7f));
                if (UnityEngine.Random.value < 0.5f)
                {
                    isMovingRight = !isMovingRight;
                    targetPoint = isMovingRight ? searchZoneRightPoint : safeZoneLeftPoint;
                    FlipDirection(isMovingRight);
                }
            }
        }




        public void PauseMovement()
        {
            isPaused = true;
        }
        public void ResumeMovement()
        {
            isPaused = false;
        }
        public void Move(Vector3 target, float speed)
        {

            target.y = transform.position.y; // Keeps the movement in the horizontal plane
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);


            //UnityEngine.Debug.Log("Moving to target: " + target + " Step: " + step);
        }
        private IEnumerable RunToSafeZone()
        {
            // Ensure the SafeZone points have been set
            if (safeZoneLeftPoint == Vector3.zero || safeZoneRightPoint == Vector3.zero)
            {
                //  Debug.LogError("SafeZone points have not been initialized.");
                yield break;
            }

            // Calculate a random position between the SafeZone points
            Vector3 randomPosition = Vector3.Lerp(safeZoneLeftPoint, safeZoneRightPoint, UnityEngine.Random.value);
            isMovingRight = randomPosition.x > transform.position.x;
            FlipDirection(isMovingRight);
            UnityEngine.Debug.Log($"Moving to random position within SafeZone: {randomPosition}");

            // Move to the random position
            while (Vector3.Distance(transform.position, randomPosition) > 1.7f)
            {
                Move(randomPosition, runSpeed);
                yield return null;
            }

            // Stop all movement once the random position is reached
            PauseMovement();

            // Wait until it's day
            while (IsNightTime())
            {
                // Optionally, you can play  perform other night-time behaviors
                yield return null;
            }

            // Resume patrolling once it's day
            UnityEngine.Debug.Log("Daytime has arrived. Archer is resuming movement.");
            ResumeMovement();

        }
        private void SetSafeZonePoints()
        {
            if (safeZone != null)
            {
                // Find "Point 1" which is a direct child of safeZone
                Transform point1Transform = safeZone.transform.Find("Point 1");
                if (point1Transform != null)
                {
                    // Set the position of Point 1
                    safeZoneLeftPoint = point1Transform.position;

                    // Now find "Point 2" which is a child of "Point 1"
                    Transform point2Transform = point1Transform.Find("Point 2");
                    Transform point3Transform = point1Transform.Find("Point 3");
                    if (point2Transform != null)
                    {
                        // Set the position of Point 2
                        safeZoneRightPoint = point2Transform.position;
                        searchZoneRightPoint = point3Transform.position;
                        UnityEngine.Debug.Log("SafeZone points initialized: Point 1 (" + safeZoneLeftPoint + "), Point 2 (" + safeZoneRightPoint + ")");
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Point 2 is not set as a child of Point 1 in the SafeZone.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Point 1 is not set as a child in the SafeZone.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("SafeZone GameObject is not assigned.");
            }
        }
        private void FlipDirection(bool facingRight)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = facingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
        private bool IsNightTime()
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            UnityEngine.Debug.Log(currentTime);
            return currentTime.Hours < 6 || currentTime.Hours >= 18;
        }
    }
}