using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsMan
{
    public class SwordsManAttack : MonoBehaviour
    {
        public DetectionZone detectionZone;
        SwordsmanController swordsmanController;
        Animator animator;

        void Awake()
        {
            swordsmanController = GetComponent<SwordsmanController>();
            animator = GetComponent<Animator>();
        }

        public void CheckDetectionZone()
        {
            List<Transform> enemies = new List<Transform>();
            foreach (Collider2D collider in detectionZone.detectedColliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    enemies.Add(collider.transform);
                }
            }

            if (enemies.Count > 0)
            {
                Transform closestEnemy = GetClosestEnemy(enemies);
                StartCoroutine(ApproachAndAttackEnemy(closestEnemy));
            }
        }

        private Transform GetClosestEnemy(List<Transform> enemies)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Transform enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        private IEnumerator ApproachAndAttackEnemy(Transform enemy)
        {
            swordsmanController.PauseMovement();
            animator.SetBool("isRunning", true);

            while (Vector3.Distance(transform.position, enemy.position) > 1f) // 1f is attack range
            {
                animator.SetBool("isAttack", false);
                swordsmanController.Move(enemy.position, swordsmanController.runSpeed);
                yield return null;
            }

            animator.SetBool("isRunning", false);
            animator.SetBool("isAttack", true);

            // Optional: Add attack logic here

            // Resume patrol after attacking
            swordsmanController.ResumeMovement();
        }
    }
}
