using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemyHealth : MonoBehaviour
{
    public float health = 100f;
    public int giveExp = 10;
    public float colorChangeDuration = 0.5f; // Duration for color change
    public ParticleSystem deathEffect; // Reference to the death particle effect
    public float deathDelay = 2f; // Time before the enemy is destroyed

    private Renderer enemyRenderer; // Reference to the Renderer
    private Collider2D enemyCollider; // Reference to the Collider
    public Boss boss;

    // Delegate for the damage event
    public delegate void DamageEventHandler(float damageAmount, Vector2 attackerPosition);
    public event DamageEventHandler OnDamageTaken;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>(); // Get the Renderer component
        enemyCollider = GetComponent<Collider2D>(); // Get the Collider component
        if (GetComponent<Boss>() != null)
        { boss = GetComponent<Boss>(); }

        // Subscribe the boss to the damage event
        if (boss != null)
        {
            OnDamageTaken += boss.OnDamageTaken;
        }
    }

    public void TakeDamage(float damageAmount, Vector2 attackerPosition)
    {
        health -= damageAmount;
        print("Took damage of " + damageAmount + " Health: " + health);

        // Trigger the damage event, passing the attacker position
        OnDamageTaken?.Invoke(damageAmount, attackerPosition);

        // Change color to red
        StartCoroutine(ChangeColor(Color.red));

        if (health <= 0)
        {
            GiveExp();
            Die();
        }
    }

    private IEnumerator ChangeColor(Color targetColor)
    {
        Color originalColor = enemyRenderer.material.color; // Save the original color
        enemyRenderer.material.color = targetColor; // Change to red

        yield return new WaitForSeconds(colorChangeDuration); // Wait for the duration

        enemyRenderer.material.color = originalColor; // Change back to original color
    }

    public void GiveExp()
    {
        FindObjectOfType<Player>().GainExperience(giveExp);
    }

    // Method to handle enemy death
    void Die()
    {

        if (boss != null)
        {
            return;
        }
        enemyCollider.enabled = false;
        enemyRenderer.enabled = false;
        // Play death particle effect
        if (deathEffect != null)
        {
            ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax); // Destroy particle system after it finishes
        }

        // Disable the renderer and collider

        // Destroy the enemy after a delay
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}
