using UnityEngine;

public class EnemyProjectileAttack : MonoBehaviour
{
    public float detectionRange = 10f; // Range within which the enemy detects the player
    public float fireRate = 1f; // How often the enemy fires
    public GameObject projectilePrefab; // The projectile prefab to be thrown
    public Transform firePoint; // The point from which the projectile is fired
    public float projectileSpeed = 5f; // Speed of the projectile

    private Transform player; // Reference to the player's transform
    private float nextFireTime = 0f; // Time until the next shot can be fired

    void Start()
    {
        player = FindObjectOfType<Player>().transform; // Find the player in the scene
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + 1f / fireRate; // Set the time for the next shot
        }
    }

    void FireProjectile()
    {
        // Instantiate the projectile at the fire point
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Calculate the direction towards the player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Set the velocity of the projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection range in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
