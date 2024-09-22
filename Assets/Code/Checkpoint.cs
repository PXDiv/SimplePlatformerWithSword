using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("cpx", transform.position.x);
            PlayerPrefs.SetFloat("cpy", transform.position.y);

            print("Player Checkpoint Set to: x" + PlayerPrefs.GetFloat("cpx") + ", " + PlayerPrefs.GetFloat("cpy"));
        }
    }
}
