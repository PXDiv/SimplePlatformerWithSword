using System;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{

    public float moveSpeed = 2f; // Normal speed of the enemy
    public float chaseSpeed = 4f; // Speed when chasing the player
    public float detectionDistance = 5f; // Distance to detect the player
    public LayerMask wallLayer; // Layer mask for wall detection

    private Rigidbody2D rb; // Reference to Rigidbody2D
    public Vector2 moveDirection = Vector2.right; // Initial move direction

    [SerializeField] BasicEnemyHealth healthSystem; // Reference to health system

    private Transform player; // Reference to the player
    private bool isChasing = false; // Flag to check if enemy is chasing

    public Vector2 startScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component

        // Attempt to get the health system component if not assigned
        if (healthSystem == null)
        {
            healthSystem = GetComponent<BasicEnemyHealth>();
        }

        // Find the player in the scene (assuming there's only one player tagged "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        startScale = transform.localScale;
        // Subscribe to the OnDamageTaken event
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken += HandleDamageTaken;
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            // Chase the player
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Check for wall collision while chasing
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionDistance, wallLayer);
            if (hit.collider == null || hit.collider.gameObject == player.gameObject)
            {
                rb.velocity = new Vector2(directionToPlayer.x * chaseSpeed, rb.velocity.y); // Use chase speed
                FaceDirection(directionToPlayer.x); // Face the direction of movement
            }
            else
            {
                // Hit a wall, stop chasing and revert to patrolling
                isChasing = false;
            }
        }
        else
        {
            // Normal patrol movement logic
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            FaceDirection(moveDirection.x); // Face the direction of patrol

            // Check for wall collision while patrolling
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, detectionDistance, wallLayer);
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                // Change direction when hitting a wall
                moveDirection = -moveDirection; // Reverse direction
                // Flip the enemy sprite here if needed
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            // Check for player detection
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, moveDirection, detectionDistance, LayerMask.GetMask("Player"));
            if (playerHit.collider != null && playerHit.collider.CompareTag("Player"))
            {
                // Start chasing the player
                isChasing = true;
            }
        }
    }

    private void HandleDamageTaken(float damageAmount, Vector2 attackerPosition)
    {
        print("Enemy took damage: " + damageAmount);
        // Add your damage handling logic here
    }

    private void FaceDirection(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1 * startScale.x, transform.localScale.y, transform.localScale.z); // Face right
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1 * startScale.x, transform.localScale.y, transform.localScale.z); // Face left
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the detection line in the scene for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDirection * detectionDistance);
    }
}
