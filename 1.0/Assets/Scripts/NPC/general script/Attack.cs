using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        Damageable damageable = collision.GetComponent<Damageable>();

        // Check if the object is damageable and not defending
        if (damageable != null && (playerController == null || !playerController.IsDefending))
        {
            bool gotHit = damageable.Hit(attackDamage, knockback);
            if (gotHit)
            {
                Debug.Log(collision.name + " hit for " + attackDamage);
            }
           
        }
    }

}
