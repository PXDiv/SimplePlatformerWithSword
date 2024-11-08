using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    public float maxGravity = 20;
    const int StartAttackDamage = 10;

    // Ground detection
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator, swordAnimator;
    [SerializeField] TMP_Text healthText, AttackText;
    [SerializeField] GameObject sword;

    // Dash variables
    public float dashSpeed = 10f; // Speed during the dash
    public float dashDuration = 0.2f; // How long the dash lasts
    public float dashCooldown = 1f; // Time before the player can dash again
    public bool aquiredDash = true;
    private bool isDashing = false;
    private bool canDash = true; // Whether the player can dash


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
    public bool doubleJumpAquired = false;

    public bool aquiredTeleball = false;
    public float teleBallThrowForce = 10f; // Adjust the force as needed
    public float teleportDelay = 0.5f;
    [SerializeField] GameObject teleBall;


    public LineRenderer pathRenderer; // Line renderer to visualize the path
    public float pathTime = 2f; // Duration for the path prediction
    private bool isHoldingThrow;

    private Renderer playerRenderer; // Reference to the Renderer
    public float colorChangeDuration = 0.5f; // Duration for color change

    [SerializeField] GameObject gameOverPanel;
    void Start()
    {
        LoadStatus();
        gameManager = FindObjectOfType<GameMan>();
        health_current = health_max;
        playerScaleStart = transform.localScale;
        playerRenderer = GetComponent<Renderer>(); // Get the Renderer component
        if (!hasSword)
        {
            sword.SetActive(false);
        }
    }

    void Update()
    {
        // Handle movement input
        moveInput = Input.GetAxis("Horizontal");

        healthText.text = health_current + "/" + health_max;
        AttackText.text = attackDamage.ToString();

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
        if (Input.GetButtonDown("Jump") && !isKnockedBack && !isDashing)
        {
            Jump();
        }

        // Handle dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing && !isKnockedBack)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetButtonDown("Fire1")) // Attack
        {
            Attack();
        }

        if (Input.GetButtonDown("Fire2") && aquiredTeleball) // Right mouse button pressed
        {
            isHoldingThrow = true;
            Time.timeScale = 0.3f; // Slow down time
        }

        if (Input.GetButtonUp("Fire2") && aquiredTeleball) // Right mouse button released
        {
            isHoldingThrow = false;
            Time.timeScale = 1f; // Restore time scale
            ThrowTeleportationBall(); // Throw the ball
            pathRenderer.positionCount = 0; // Clear the path
        }

        // If holding the throw button, show the path prediction
        if (isHoldingThrow && aquiredTeleball)
        {
            ShowPathPrediction();
        }
    }


    void FixedUpdate()
    {
        if (!isKnockedBack && !isDashing)
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

            if (!isGrounded && rb.gravityScale < maxGravity)
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
        else if (canDoubleJump && !doubleJumpEnabled && doubleJumpAquired) // Double jump logic
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
    public int attackDamage;
    public float knockBackOnFire = 1;
    public bool hasSword = false;

    public void Attack()
    {
        if (hasSword)
        {
            // Detect enemies in range of attack
            sword.gameObject.SetActive(true);
            swordAnimator.SetTrigger("Attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Hitable);

            //sword.LeanMoveLocal(new Vector2(0.3f, sword.transform.localPosition.y), 0.2f).setFrom(new Vector3(0.1f, -0.18f, 0)).setEaseOutExpo();
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

        // Change color to red when taking damage
        StartCoroutine(ChangeColor(Color.red));

        if (health_current <= 0)
        {
            Die();
        }
    }

    //-----------------

    public void Die()
    {
        RespawnFromCheckpoint();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RespawnFromCheckpoint()
    {
        print("Reswpawn");
        health_current = health_max;
        transform.position = new Vector2(PlayerPrefs.GetFloat("cpx"), PlayerPrefs.GetFloat("cpy"));

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

        // Apply the knockback force in both horizontal and vertical directions
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

    // Coroutine to handle color change
    private IEnumerator ChangeColor(Color targetColor)
    {
        Color originalColor = playerRenderer.material.color; // Save the original color
        playerRenderer.material.color = targetColor; // Change to red

        yield return new WaitForSeconds(colorChangeDuration); // Wait for the duration

        playerRenderer.material.color = originalColor; // Change back to original color
    }

    // Dash
    IEnumerator Dash()
    {
        if (aquiredDash)
        {
            canDash = false;
            isDashing = true;

            float originalGravity = rb.gravityScale; // Save original gravity scale
            rb.gravityScale = 0; // Disable gravity during dash

            Vector2 dashDirection = new Vector2(transform.localScale.x, 0).normalized;
            rb.velocity = dashDirection * dashSpeed; // Apply dash velocity

            yield return new WaitForSeconds(dashDuration); // Wait for dash duration

            rb.gravityScale = originalGravity; // Restore gravity scale
            isDashing = false;

            yield return new WaitForSeconds(dashCooldown); // Wait for cooldown

            canDash = true;
        }
    }

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
        print("max health upgraded");
        health_current += points;
        health_max += points;
        gameManager.ResumeGame();
        Time.timeScale = 1;
        healthSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(health_max, 20);
        SaveStatus();
    }

    public void UpgradeArmor(int points)
    {
        // Armor upgrade implementation here
        gameManager.ResumeGame();
    }

    public void UpgradeDamage(int points)
    {
        attackDamage += points;
        gameManager.ResumeGame();
        SaveStatus();

    }

    public void SaveStatus()
    {
        PlayerPrefs.SetInt("AttackDamage", attackDamage);
        PlayerPrefs.SetFloat("Health", health_max);
        PlayerPrefs.SetInt("Level", playerLevel);
    }

    public void LoadStatus()
    {
        attackDamage = PlayerPrefs.GetInt("AttackDamage", 10);
        if (PlayerPrefs.GetFloat("Health", 100) < 99)
        {
            PlayerPrefs.SetFloat("Health", 100);
        }
        health_max = PlayerPrefs.GetFloat("Health", 100);
        playerLevel = PlayerPrefs.GetInt("Level", 1);

    }
}
