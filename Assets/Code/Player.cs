using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Variables for movement
    public float moveSpeed = 5f;
    public float jumpForce = 1f;

    // Player Variables 
    public int playerLevel = 1;
    public int playerExp = 0;
    public float health_max = 100, health_current;
    public float massIncrements = 1f;
    const int StartAttackDamage = 10;

    // Ground detection
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] TMP_Text debugText;
    [SerializeField] GameObject sword;

    private bool isGrounded;
    private bool isJumping;
    private float moveInput;
    Vector2 playerScaleStart;
    private GameMan gameManager;


    public float knockbackForce = 10f; // Knockback force to be applied
    public float knockbackDuration = 0.2f; // Duration of the knockback effect
    private bool isKnockedBack;

    // Double jump variables
    public bool canDoubleJump = false; // Can the player double jump
    public bool doubleJumpEnabled = false; // Has the player used the double jump

    public float teleBallThrowForce = 10f; // Adjust the force as needed
    public float teleportDelay = 0.5f;
    [SerializeField] GameObject teleBall;

    public LineRenderer pathRenderer; // Line renderer to visualize the path
    public float pathTime = 2f; // Duration for the path prediction
    private bool isHoldingThrow;

    void Start()
    {
        gameManager = FindObjectOfType<GameMan>();
        health_current = health_max;
        playerScaleStart = transform.localScale;
    }
    void Update()
    {
        // Handle movement input
        moveInput = Input.GetAxis("Horizontal");

        debugText.text = "health: " + health_current + " Attack: " + attackDamage + "<Br>Exp: " + playerExp + " Level: " + playerLevel;

        healthSlider.value = health_current;
        healthSlider.maxValue = health_max;

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset double jump status when grounded
        if (isGrounded)
        {
            canDoubleJump = true;
            doubleJumpEnabled = false;
        }

        if (health_current <= 0)
        {
            Die();
        }

        // Handle jump input
        if (Input.GetButtonDown("Jump") && !isKnockedBack)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1")) // Attack
        {
            Attack();
        }

        if (Input.GetButtonDown("Fire2")) // Right mouse button pressed
        {
            isHoldingThrow = true;
            Time.timeScale = 0.3f; // Slow down time
        }

        if (Input.GetButtonUp("Fire2")) // Right mouse button released
        {
            isHoldingThrow = false;
            Time.timeScale = 1f; // Restore time scale
            ThrowTeleportationBall(); // Throw the ball
            pathRenderer.positionCount = 0; // Clear the path
        }

        // If holding the throw button, show the path prediction
        if (isHoldingThrow)
        {
            ShowPathPrediction();
        }
    }
    void FixedUpdate()
    {
        if (!isKnockedBack)
        {
            float horizontalForce = moveInput * moveSpeed;

            if (moveInput < 0)
            {
                transform.localScale = new Vector2(-1 * playerScaleStart.x, transform.localScale.y);
            }

            if (moveInput > 0)
            {
                transform.localScale = new Vector2(1 * playerScaleStart.x, transform.localScale.y);
            }

            if (moveInput != 0)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            // Limit horizontal speed if necessary
            rb.velocity = new Vector2(Mathf.Clamp(horizontalForce, -moveSpeed, moveSpeed), rb.velocity.y);

            if (!isGrounded)
            {
                rb.gravityScale += massIncrements;
            }
            else
            {
                rb.gravityScale = 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the ground check circle in the scene view for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Jump()
    {
        if (isGrounded)
        {
            animator.SetTrigger("Jump");
            rb.gravityScale = 1;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = false;
            canDoubleJump = true; // Allow double jump after the first jump
        }
        else if (canDoubleJump && !doubleJumpEnabled) // Double jump logic
        {
            animator.SetTrigger("Jump");
            rb.gravityScale = 1;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            doubleJumpEnabled = true; // Mark double jump as used
        }
    }

    //Attack-----------------
    public float attackRange = 0.5f;
    public LayerMask Hitable;
    [SerializeField] Transform attackPoint;
    public float attackDamage;
    public float knockBackOnFire = 1;
    public bool hasSword = false;

    public void Attack()
    {
        if (hasSword)
        {        // Detect enemies in range of attack
            sword.gameObject.SetActive(true);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Hitable);

            //sword.LeanRotate(new Vector3(0, 0, 270), 0.5f).setEaseOutExpo().setFrom(new Vector3(0, 0, 350));
            sword.LeanMoveLocal(new Vector2(0.3f, sword.transform.localPosition.y), 0.2f).setFrom(new Vector3(0.1f, -0.18f, 0)).setEaseOutExpo();
            // Damage them
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<BasicEnemyHealth>().TakeDamage(attackDamage, transform.position);
                ApplyKnockback(attackPoint.position, knockBackOnFire);
            }
        }
        else
        {
            sword.gameObject.SetActive(false);
        }
    }

    //-----------------

    //Health Functions
    [SerializeField] Slider healthSlider;
    public void HealPlayer(int points)
    {
        health_current += points;

        if (health_current > health_max)
        {
            health_current = health_max;
        }
    }
    public void MaxHealPlayer()
    {
        health_current = health_max;
    }
    public void ReduceHealth(float reduceHealthBy, Transform enemyPosition)
    {
        health_current -= reduceHealthBy;
        ApplyKnockback(enemyPosition.position, knockbackForce);
    }

    //-----------------

    public void Die()
    {
        gameManager.RestartLevel();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            Die();
        }
    }

    //KnockBack-------------------
    void ApplyKnockback(Vector3 enemyPosition, float kbForce)
    {
        if (isKnockedBack) return;

        Vector2 knockbackDirection = (transform.position - enemyPosition).normalized;

        // Apply the knockback force in both horizo ntal and vertical directions
        rb.velocity = knockbackDirection * kbForce;
        StartCoroutine(KnockbackCoroutine());
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }
    //---------------------------

    //Teleportation Ball System

    public void ThrowTeleportationBall()
    {
        // Instantiate the ball at the player's position
        GameObject ball = Instantiate(teleBall, transform.position, Quaternion.identity);
        var rg = ball.GetComponent<Rigidbody2D>();

        // Get the mouse position in the world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the player to the mouse position
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Optionally, set a speed for the ball

        // Apply the force to the ball's Rigidbody2D
        rg.AddForce(direction * teleBallThrowForce, ForceMode2D.Impulse);
    }
    private void ShowPathPrediction()
    {
        if (pathRenderer != null)
        {
            pathRenderer.positionCount = 0; // Reset positions

            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;

            // Predict the path for the specified duration
            Vector2 startPosition = (Vector2)transform.position;
            Vector2 velocity = direction * teleBallThrowForce;

            // Draw the predicted path
            for (float t = 0; t <= pathTime; t += 0.1f) // Increment by small intervals
            {
                Vector2 position = startPosition + velocity * t + 0.5f * Physics2D.gravity * t * t;
                pathRenderer.positionCount++;
                pathRenderer.SetPosition(pathRenderer.positionCount - 1, position);
            }
        }
    }
    //---------------------------


    // Level Systems -------------------------------------

    public int[] xpForNextLevel;
    [SerializeField] GameObject upgradesPanel;
    public void GainExperience(int exp)
    {
        playerExp += exp;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if (playerLevel < xpForNextLevel.Length && playerExp >= xpForNextLevel[playerLevel - 1])
        {
            playerExp -= xpForNextLevel[playerLevel - 1];
            playerLevel++;
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        upgradesPanel.SetActive(true);
    }

    //---------------------------------------------
    // Upgrade Systems -------------------------------------

    public void UpgradeMaxHealth(int points)
    {
        health_current += points;
        health_max += points;
        gameManager.ResumeGame();
        Time.timeScale = 1;
        healthSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(health_max, 20);
        print("upg");
    }
    public void UpgradeArmor(int points)
    {
        // += points;
        gameManager.ResumeGame();
        print("upg");

    }
    public void UpgradeDamage(int points)
    {
        attackDamage += points;
        gameManager.ResumeGame();
        print("upg");

    }
}
