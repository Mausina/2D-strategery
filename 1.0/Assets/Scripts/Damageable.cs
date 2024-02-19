﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damagebleHit;
    [SerializeField]
    public int _maxHealth = 100;
    Animator animator;


    public int MaxHealth
    {
        get
        { return _maxHealth; }
        set { _maxHealth = value; }
    }
    private int _minHealth = 100;
    public int Health
    {
        get { return _minHealth; }
        set
        {
            _minHealth = value;
            if (_minHealth <= 0)
            {
                IsAlive = false;

            }

        }
    }
    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;



    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }

        set
        {

            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set" + value);
        }

    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.LockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.LockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {

            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;

            }
            timeSinceHit += Time.deltaTime;
        }


    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (_isAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTriger);
            LockVelocity = true;
            damagebleHit?.Invoke(damage, knockback);


            return true;
        }

        return false;
    }

}