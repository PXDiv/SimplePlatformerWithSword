using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageOnTouch : MonoBehaviour
{
    private float enemyTouchDamage = 10f;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            other.gameObject.GetComponent<Player>().ReduceHealth(enemyTouchDamage, transform);
        }
    }
}
