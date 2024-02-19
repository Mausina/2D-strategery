using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDenonController : MonoBehaviour
{
    public DetectionZone zone;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float force;
    public float timer;
    public float waypointReachedDistance = 0.1f;
    public bool _hasTarget = false;
    public List<Transform> waypoints;
    public float flightSpeed = 2f;

    private GameObject player;
    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWaypoint;
    int waypointNum = 0;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set { _hasTarget = value; animator.SetBool(AnimationStrings.hasTarget, value); }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        damageable = GetComponent<Damageable>();
        nextWaypoint = waypoints[waypointNum];
    }

    void Update()
    {
        HasTarget = zone.detectedColliders.Count > 0;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        

        if (HasTarget && distance < 10)
        {
            timer += Time.deltaTime;
            if (timer > 2)
            {
                Shoot();
                timer = 0;  // Reset timer after shooting
            }
        }
        else
        {
            // Reset timer if distance is greater than or equal to 10
            timer = 0;
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }


    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Flight()
    {
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector2.Distance(nextWaypoint.position, transform.position);
        
        rb.velocity = directionToWaypoint * flightSpeed;

        if (distance <= waypointReachedDistance)
        {

            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                
                waypointNum = 0;
            }


            nextWaypoint = waypoints[waypointNum];
            Flip();
        }
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    void Shoot()
    {
        // Instantiate the fireball at the firePoint's position and rotation
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        // Assuming fireballPrefab has a Rigidbody2D component
        Rigidbody2D fireballRB = fireball.GetComponent<Rigidbody2D>();

        // Ensure fireballRB is not null before setting velocity
        if (fireballRB != null)
        {
            Vector3 direction = (player.transform.position - firePoint.position).normalized;
            fireballRB.velocity = direction * force;
        }
    }

}

