using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0649, 0414

public class Log : MonoBehaviour
{
    bool isBlocking = false;
    [Header("Log falls north by default.  To cross path, Adjust Y or rotate in scene")]
    //[SerializeField] Vector3 blockingAngle = new Vector3(90, 0, 0);
    [SerializeField] AudioClip sfx;
    [SerializeField] float fearDistanceFromTreeFall = 4f;


    public bool IsBlocking()
    {
        return isBlocking;
    }
    public void Block()
    {
        isBlocking = true;
        transform.rotation = isBlocking ? Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0): Quaternion.identity;
        GetComponent<NavMeshObstacle>().enabled = true;
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfx);
        GameObject.FindGameObjectWithTag("MiniMap").GetComponent<MiniMap2>().UpdateLogs();
        tag = "BlockingTree";
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().RemoveThing(gameObject);

        Enemy.ScareNearbyEnemies(transform.position, fearDistanceFromTreeFall);

    }
}
