using System;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of the enemy
    //public int moveDir = 1; 
    public float detectionDistance = 0.5f; // Distance to detect walls
    public LayerMask wallLayer; // Layer mask for wall detection

    private Rigidbody2D rb; // Reference to Rigidbody2D
    public Vector2 moveDirection = Vector2.right; // Initial move direction

    [SerializeField] BasicEnemyHealth healthSystem; // Reference to health system

    private bool isKnockback = false; // Flag to check if enemy is in knockback state
    public float knockbackDuration = 0.5f; // Duration of the knockback effect
    private float knockbackEndTime; // Time when knockback should end
    public float knockbackPower = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component

        // Attempt to get the health system component if not assigned
        if (healthSystem == null)
        {
            healthSystem = GetComponent<BasicEnemyHealth>();
        }

        // Subscribe to the OnDamageTaken event
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken += HandleDamageTaken;
        }
    }

    private void HandleDamageTaken(float damageAmount, Vector2 attackerPosition)
    {
        print("Enemy took damage: " + damageAmount);

        // Calculate knockback direction
        Vector2 knockbackDirection = (transform.position - (Vector3)attackerPosition).normalized;

        // Set knockback velocity
        rb.velocity =  knockbackDirection * knockbackPower; // Adjust the multiplier for the desired knockback strength

        // Activate knockback state
        isKnockback = true;
        knockbackEndTime = Time.time + knockbackDuration; // Set the end time for knockback
    }

    void FixedUpdate()
    {
        if (isKnockback)
        {
            // Check if knockback duration has ended
            if (Time.time >= knockbackEndTime)
            {
                isKnockback = false; // Reset the knockback state
            }
            // No normal movement if in knockback state
            return;
        }

        // Normal movement logic
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        // Check for wall collision
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, detectionDistance, wallLayer);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            // Change direction when hitting a wall
            moveDirection = -moveDirection; // Reverse direction
            // Flip the enemy sprite here if needed
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the detection line in the scene for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDirection * detectionDistance);
    }
}
