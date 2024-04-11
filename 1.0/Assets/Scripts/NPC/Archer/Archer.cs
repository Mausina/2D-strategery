
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
        private GameObject searchZone;
        private GameObject safeZone;
        private bool isMovingRight = true;
        private Vector3 originalScale; // To maintain the original local scale
        WorldPoolManager poolManager;
        void Start()
        {
            animator = GetComponent<Animator>();
            originalScale = transform.localScale;
            StartCoroutine(BehaviorController());
        }
        
// This method will be called by the WorldPoolManager when the archer is registered or deregistered
public void AssignPool(WorldPoolManager newPool)
{
     poolManager = newPool;
}

        public void SetSafeZone(GameObject zone)
        {
            safeZone = zone;
        }

        public void SetSearchZone(GameObject zone)
        {
            searchZone = zone;
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

        private IEnumerator Patrol()
        {
            float patrolTime = UnityEngine.Random.Range(5f, 10f);

            animator.SetBool("isMoving", true);
            float endTime = Time.time + patrolTime;

            while (Time.time < endTime)
            {
                Move(patrolSpeed);
                yield return null;

                // Check if the archer has reached the edge of the SearchZone
                if (ReachedEdgeOfSearchZone())
                {
                    TurnAround();
                }
            }

            animator.SetBool("isMoving", false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));

            if (UnityEngine.Random.value < 0.4f) // 40% chance to turn around
            {
                TurnAround();
            }
        }

        private IEnumerator RunToSafeZone()
        {
            animator.SetBool("isRun", true);

            while (Vector3.Distance(transform.position, safeZone.transform.position) > 0.1f)
            {
                Move(runSpeed);
                yield return null;
            }

            animator.SetBool("isRun", false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
        }

        private void Move(float speed)
        {
            float step = speed * Time.deltaTime;
            transform.position += isMovingRight ? new Vector3(step, 0, 0) : new Vector3(-step, 0, 0);
        }

        private void TurnAround()
        {
            isMovingRight = !isMovingRight;
            Vector3 flippedScale = originalScale;
            flippedScale.x *= isMovingRight ? 1 : -1;
            transform.localScale = flippedScale;
        }

        private bool IsNightTime()
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            return currentTime.Hours < 6 || currentTime.Hours >= 18;
        }

        private bool ReachedEdgeOfSearchZone()
        {
            // Simple example assuming searchZone is a BoxCollider2D
            BoxCollider2D collider = searchZone.GetComponent<BoxCollider2D>();
            if (!collider) return false;

            float rightEdge = searchZone.transform.position.x + collider.size.x / 2;
            float leftEdge = searchZone.transform.position.x - collider.size.x / 2;

            // Turn around when reaching the edges
            if (transform.position.x >= rightEdge || transform.position.x <= leftEdge)
            {
                return true;
            }
            return false;
        }
    }
}

