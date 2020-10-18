using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Branch : MonoBehaviour
{
    bool isBlocking = false;
    [Header("Branch falls north by default.  To cross path, Adjust Y or rotate in scene")]
    [SerializeField] Vector3 blockingAngle = new Vector3(90, 0, 0);
    [SerializeField] AudioClip sfx;

    // get/set branch blocking the path
    public void Block()
    {
        isBlocking = true;
        transform.rotation = isBlocking ? Quaternion.Euler(blockingAngle): Quaternion.identity;
        GetComponent<NavMeshObstacle>().enabled = true;
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfx);
    }
}
