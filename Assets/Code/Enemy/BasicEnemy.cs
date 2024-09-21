using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHealth : MonoBehaviour
{
    public float health = 100f;
    public int giveExp = 10;

    // Delegate for the damage event
    public delegate void DamageEventHandler(float damageAmount, Vector2 attackerPosition);
    public event DamageEventHandler OnDamageTaken;

    public void TakeDamage(float damageAmount, Vector2 attackerPosition)
    {
        health -= damageAmount;
        print("Took damage of " + damageAmount + " Health: " + health);

        // Trigger the damage event, passing the attacker position
        OnDamageTaken?.Invoke(damageAmount, attackerPosition);

        if (health <= 0)
        {
            GiveExp();
            Die();
        }
    }

    public void GiveExp()
    {
        FindObjectOfType<Player>().GainExperience(giveExp);
    }

    // Method to handle enemy death
    void Die()
    {
        // Add death effects/animations here
        Destroy(gameObject);
    }
}
