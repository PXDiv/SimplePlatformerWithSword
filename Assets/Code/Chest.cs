using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite chestOpen;
    public string title;
    public string message;
    bool hasOpened;

    public bool enableSword, enableDoubleJump, enableDash, enableTeleBall;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hasOpened)
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        hasOpened = true;
        GetComponent<SpriteRenderer>().sprite = chestOpen;
        FindObjectOfType<MessagePanel>().SendMessage(title, message);

        var player = FindObjectOfType<Player>();

        if (enableDoubleJump)
        {
            player.doubleJumpAquired = true;
        }
        if (enableSword)
        {
            player.hasSword = true;
        }


        if (enableDash)
        {
            player.aquiredDash = true;
        }
        if (enableTeleBall)
        {
            player.aquiredTeleball = true;
        }
    }
}
