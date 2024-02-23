using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using WorldTime;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpInpulse = 5f;
    public int maxHealth = 100;
    public HealthBar healthBar;
    Vector2 moveInput;
    Damageable damageable;
    private bool _isDefending = false;
    [SerializeField]
    WorldTimeSystem.WorldTime myWorldTime;
    TouchingDirection touchingDirection;


    public float maxStamina = 30f; // Maximum stamina
    private float currentStamina; // Current stamina level
    private bool isExhausted = false; // Is the player exhausted?


    private void Update()
    {
        HandleStamina();
        UpdateDyspneaAnimation();
    }
    private void HandleStamina()
    {
        if (IsRunning)
        {
            // Decrease stamina while running
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                isExhausted = true; // Player becomes exhausted
                currentStamina = 0; // Ensure stamina does not go below 0
            }
        }
        else
        {
            // Recover stamina when not running
            if (currentStamina < maxStamina)
            {
                currentStamina += Time.deltaTime; // Adjust recovery rate as needed
            }
            else if (isExhausted && currentStamina >= maxStamina / 2) // Half stamina needed to recover from exhaustion
            {
                isExhausted = false; // Player recovers from exhaustion
            }
        }

        // Limit player's ability to run if exhausted
        if (isExhausted)
        {
            LimitPlayerMovement();
        }

        // Update UI or indicators of stamina/exhaustion here if necessary
    }

    private void LimitPlayerMovement()
    {
        // Limit the player's speed to walking speed when exhausted
        if (IsRunning)
        {
            IsRunning = false; // Force player to walk
            // Optionally, apply a visual or audio cue to indicate exhaustion
        }
    }

    private void UpdateDyspneaAnimation()
    {
        // Update animator with dyspnea or exhaustion animation based on isExhausted
        animator.SetBool("IsDyspneic", isExhausted);
    }


















    public bool IsDefending
{
    get { return _isDefending; }
}


    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirection.IsOnWall)
                {
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else { return walkSpeed; }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }

    [SerializeField]
    public bool _isMoving = false;

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {

            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }



    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }


    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
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

    Rigidbody2D rb;
    Animator animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
        currentStamina = maxStamina;
    }

    private void Start()
    {
        myWorldTime = FindObjectOfType<WorldTimeSystem.WorldTime>();
        healthBar.SetMaxHealth(maxHealth);
        if (myWorldTime != null)
        {
            myWorldTime.WorldTimeChanged += OnWorldTimeChanged;
        }
        else
        {
            Debug.LogError("myWorldTime is not assigned on " + gameObject.name);
        }

    }
    private void OnDestroy()
    {
        if (myWorldTime != null)
        {
            myWorldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {

            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed * Time.fixedDeltaTime, rb.velocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.magnitude);

    }



    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            moveInput = context.ReadValue<Vector2>();
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            moveInput = Vector2.zero;
            IsMoving = false;
        }

    }


    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }


    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.isGround && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpInpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attack);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);

        maxHealth -= damage;
        if (maxHealth < 0)
            maxHealth = 0;
        healthBar.SetHealth(maxHealth);
    }


    public void OnDefense(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            _isDefending = true;
            animator.SetTrigger(AnimationStrings.defenseTriger);
        }
        else if (context.canceled)
        {
            _isDefending = false;
        }
    }
    private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
    {
        // Determine if it's day or night based on currentTime
        // Assuming day is between 6:00 AM (06:00) and 6:00 PM (18:00)
        bool isNight = currentTime.Hours < 6 || currentTime.Hours >= 19;
        
        // Set the animator boolean parameter "IsNight" based on the isNight condition
        animator.SetBool(AnimationStrings.isNightBool, isNight);
       // Debug.Log($"Time changed. Current hour: {currentTime.Hours}, IsNight: {isNight}");

    }


}
