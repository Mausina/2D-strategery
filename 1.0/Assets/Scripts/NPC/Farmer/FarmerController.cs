using System.Collections;
using UnityEngine;
namespace Farmer
{
    public class FarmerController : MonoBehaviour
    {
        private GameObject campFireZone;
        public Transform pointA;
        public Transform pointB;
        private Transform currentTarget;
        private bool isMovingToB = true;
        private float speed = 5f;
        private bool insideZone = false;  // Flag to check if the farmer is inside the zone
        private float nextPauseTime = 0;  // Time when the next pause may occur
        private float timeBetweenPauses = 2.5f;  // Minimum time between pauses
        private Vector3 lastTurnPosition; // Keep track of the position at the last turn
        private float minimumMoveDistance = Random.Range(1f, 3f); // Minimum distance to move before turning again
        private bool isTurning = false;
        public void SetCampFireZone(GameObject zone)
        {
            campFireZone = zone;
            pointA = campFireZone.transform.GetChild(0);
            pointB = campFireZone.transform.GetChild(1);
            currentTarget = pointA;  // Initialize the first target
            lastTurnPosition = transform.position; // Initialize last turn position
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == campFireZone)
            {
                insideZone = true;
                StartCoroutine(MoveBetweenPoints());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == campFireZone)
            {
                insideZone = false;
                StopAllCoroutines();
            }
        }

        private IEnumerator MoveBetweenPoints()
        {

            while (insideZone)
            {
                while (Vector3.Distance(transform.position, currentTarget.position) > 0.5f)
                {
                    MoveTowardsTarget();

                    if (Time.time >= nextPauseTime && Vector3.Distance(transform.position, lastTurnPosition) > minimumMoveDistance)
                    {
                        if (Random.Range(0, 100) < 20)  // 20% chance to pause
                        {
                            float pauseTime = Random.Range(2f, 6f);
                            yield return new WaitForSeconds(pauseTime);

                            if (Random.Range(0, 2) > 0) // 50% chance to turn around after pausing
                            {
                                isMovingToB = !isMovingToB;
                                currentTarget = isMovingToB ? pointB : pointA;
                                FlipDirection();
                                lastTurnPosition = transform.position; // Update last turn position
                            }

                            nextPauseTime = Time.time + timeBetweenPauses; // Set next pause time
                        }
                    }

                    yield return null;
                }

                float stopTime = Random.Range(2f, 4f);
                yield return new WaitForSeconds(stopTime);

                if (Random.Range(0, 2) > 0) // 50% chance to turn around after reaching the target
                {
                    isMovingToB = !isMovingToB;
                    currentTarget = isMovingToB ? pointB : pointA;
                    FlipDirection();
                    lastTurnPosition = transform.position; // Update last turn position
                }
            }
        }

        private void FlipDirection()
        {
            // Adjust the facing direction of the NPC based on the target
            Vector3 localScale = transform.localScale;
            if ((currentTarget == pointB && localScale.x < 0) || (currentTarget == pointA && localScale.x > 0))
            {
                localScale.x *= -1; // Flip the x-scale to face the new direction
                transform.localScale = localScale;
            }
        }
        /*
        private IEnumerator SmoothTurn()
        {
            if (isTurning)
                yield break; // Exit if we're already turning to prevent overlapping turns.

            isTurning = true;

            float turnDuration = 0.5f; // Duration to smoothly turn
            float timer = 0;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(-startScale.x, startScale.y, startScale.z); // Flip x-scale to turn

            while (timer < turnDuration)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, timer / turnDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale; // Ensure the scale is fully set to end scale
            lastTurnPosition = transform.position; // Update last turn position

            isTurning = false; // Turning is complete
        }
        */
        private void MoveTowardsTarget()
        {
            FlipDirection();
            Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
            if (GetComponent<Rigidbody2D>() != null)
            {
                GetComponent<Rigidbody2D>().MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }
        }
    }
}

