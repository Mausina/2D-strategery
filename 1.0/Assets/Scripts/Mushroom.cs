using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class Mushroom : MonoBehaviour
{
    public float walkSpeed = 3f;

    Rigidbody2D rb;
    TouchingDirection touchingDirection;

    public enum WalkableDirection { Right, Left }


    public WalkableDirection dir;
    private Vector2 WalkableDirectionVector = Vector2.right;


    public WalkableDirection WalkDirection
    {
        get { return dir; }
        set
        {
            if (dir != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    WalkableDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)

                { WalkableDirectionVector = Vector2.left; }



            dir = value;
            }

        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
    }

    private void FixedUpdate()
    {

        if (touchingDirection.IsOnWall && touchingDirection.isGround)
        {
            FlipDirection();
        }
        rb.velocity = new Vector2(walkSpeed * WalkableDirectionVector.x, rb.velocity.y);
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        { WalkDirection = WalkableDirection.Right; }
        else
        {
            Debug.LogError("Current walkable direction is not set to legal values of right or left");
        }

    }

    
}