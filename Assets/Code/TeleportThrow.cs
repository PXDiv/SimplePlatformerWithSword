using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportThrow : MonoBehaviour
{
    GameObject player;
    [SerializeField] Rigidbody2D rg;

    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        rg.isKinematic = true;
        rg.velocity = Vector2.zero;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(player.GetComponent<Player>().teleportDelay);
        player.transform.position = transform.position;
        Destroy(gameObject);
    }
}
