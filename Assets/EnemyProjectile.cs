using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float projectileDamage = 5f;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            other.gameObject.GetComponent<Player>().ReduceHealth(projectileDamage, transform);

        }
        GetComponent<Collider2D>().enabled = false;
        transform.LeanScale(Vector2.zero, 0.5f).setEaseOutExpo();
        StartCoroutine(DestroyAfterDelay());
    }
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
