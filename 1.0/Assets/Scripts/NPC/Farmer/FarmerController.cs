using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Farmer
{
    public class FarmerController : MonoBehaviour
    {
        private GameObject campFireZone;
        private GameObject field;
        public Transform pointA;
        public Transform pointB;
        private Transform currentTarget;
        private bool isMovingToB = true;
        private float speed = 5f;
        private bool insideZone = false;
        private float nextPauseTime = 0;
        private float timeBetweenPauses = 2.5f;
        private Vector3 lastTurnPosition;
        private float minimumMoveDistance = Random.Range(1f, 3f);
        private bool isCoroutineRunning = false;
        private Coroutine movingBetweenPointsCoroutine = null;
        public void SetCampFireZone(GameObject zone)
        {
            campFireZone = zone;
            pointA = campFireZone.transform.GetChild(0);
            pointB = campFireZone.transform.GetChild(1);
            currentTarget = pointA;
            lastTurnPosition = transform.position;
        }

        public void SetField(GameObject field)
        {
            this.field = field;
        }

        private void Update()
        {
            if (IsNightTime())
            {
                if (!insideZone)
                {
                    MoveTowards(campFireZone.transform.position); // Move towards the campfire zone at night if outside
                }
                else if (!isCoroutineRunning)
                {
                    StartCoroutine(MoveBetweenPoints()); // Start moving between points at night if inside zone
                }
            }
            else // Daytime logic
            {
                if (field != null) // If there is a field
                {
                    StopCoroutineIfNeeded();
                    MoveTowards(field.transform.position); // Move towards the field
                }
                else if (!isCoroutineRunning && insideZone) // No field, and coroutine is not running, ensure it's day and inside zone
                {
                    StartCoroutine(MoveBetweenPoints());
                }
            }
        }

        private bool IsNightTime()
        {
            TimeSpan currentTime = WorldTimeSystem.WorldTime.Instance.GetCurrentTime();
            return currentTime.Hours < 6 || currentTime.Hours >= 18;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == campFireZone)
            {
                insideZone = true;
                if (IsNightTime())
                {
                    movingBetweenPointsCoroutine = StartCoroutine(MoveBetweenPoints());
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == campFireZone)
            {
                insideZone = false;
                StopCoroutine(MoveBetweenPoints());
                isCoroutineRunning = false;
            }
        }

        private IEnumerator MoveBetweenPoints()
        {
            isCoroutineRunning = true;
            while (insideZone)
            {
                while (Vector3.Distance(transform.position, currentTarget.position) > 2f)
                {
                    MoveTowards(currentTarget.position);
                    if (Time.time >= nextPauseTime && Vector3.Distance(transform.position, lastTurnPosition) > minimumMoveDistance)
                    {
                        yield return PauseAndPotentiallyTurn();
                    }
                    yield return null;
                }
                yield return PauseAtTarget();
            }
            isCoroutineRunning = false;
        }

        private void StopCoroutineIfNeeded()
        {
            if (isCoroutineRunning)
            {
                StopAllCoroutines();
                isCoroutineRunning = false; // Reset the coroutine flag
            }
        }

        private IEnumerator PauseAndPotentiallyTurn()
        {
            if (Random.Range(0, 100) < 20)
            {
                float pauseTime = Random.Range(2f, 6f);
                yield return new WaitForSeconds(pauseTime);
                if (Random.Range(0, 2) > 0)
                {
                    isMovingToB = !isMovingToB;
                    currentTarget = isMovingToB ? pointB : pointA;
                    FlipDirection();
                    lastTurnPosition = transform.position;
                }
                nextPauseTime = Time.time + timeBetweenPauses;
            }
        }

        private IEnumerator PauseAtTarget()
        {
            float stopTime = Random.Range(2f, 4f);
            yield return new WaitForSeconds(stopTime);
            if (Random.Range(0, 2) > 0)
            {
                isMovingToB = !isMovingToB;
                currentTarget = isMovingToB ? pointB : pointA;
                FlipDirection();
                lastTurnPosition = transform.position;
            }
        }

        private void MoveTowards(Vector3 targetPosition)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (GetComponent<Rigidbody2D>() != null)
            {
                GetComponent<Rigidbody2D>().MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }
            FlipDirection();
        }

        private void FlipDirection()
        {
            Vector3 localScale = transform.localScale;
            if ((currentTarget == pointB && localScale.x < 0) || (currentTarget == pointA && localScale.x > 0))
            {
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }
    }
}
