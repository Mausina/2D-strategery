

using System.Collections;
using UnityEngine;

namespace Archer
{
    public class ArcherController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float runSpeed = 4f;

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
            float patrolTime = Random.Range(5f, 10f);

            // Start patrol
            animator.SetBool("isMoving", true);
            float endTime = Time.time + patrolTime;

            while (Time.time < endTime)
            {
                Move(patrolSpeed);
                yield return null;
            }

            // Stop and possibly turn around
            animator.SetBool("isMoving", false);
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            if (Random.value < 0.4f) // 40% chance to turn around
            {
                TurnAround();
            }
        }

        private IEnumerator RunToSafeZone()
        {
            // Trigger running animation
            animator.SetBool("isRun", true);

            // Logic to move towards the safe zone, for demonstration we move in a straight line
            while (Vector3.Distance(transform.position, safeZone.transform.position) > 0.1f)
            {
                Move(runSpeed);
                yield return null;
            }

            // Stop running
            animator.SetBool("isRun", false);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }

        private void Move(float speed)
        {
            // Simple movement logic - adjust direction based on isMovingRight
            float step = speed * Time.deltaTime;
            transform.position += isMovingRight ? new Vector3(step, 0, 0) : new Vector3(-step, 0, 0);
        }

        private void TurnAround()
        {
            isMovingRight = !isMovingRight;
            // Flip the scale to turn around
            Vector3 flippedScale = originalScale;
            flippedScale.x *= isMovingRight ? 1 : -1;
            transform.localScale = flippedScale;
        }

        private bool IsNightTime()
        {
            // Placeholder for night time check
            var currentTime = System.DateTime.Now;
            return currentTime.Hour < 6 || currentTime.Hour >= 18;
        }
    }
}
