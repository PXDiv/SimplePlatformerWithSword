using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Shower : MonoBehaviour
{
    public Transform[] transforms;
    public GameObject[] enemiesToSpawn;
    public GameObject boulder;

    public void DoShower()
    {
        foreach (Transform t in transforms)
        {
            print("Doing Showers");
            Instantiate(boulder, t.position, quaternion.identity);
        }
    }

    public void DoEnemyShower()
    {
        foreach (Transform t in transforms)
        {
            print("Doing Showers");
            Instantiate(enemiesToSpawn[UnityEngine.Random.Range(0, enemiesToSpawn.Length)], t.position, quaternion.identity);
        }
    }
}
