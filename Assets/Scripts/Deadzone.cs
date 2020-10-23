using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    PlayerController player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        Debug.Log("******* Add " + other.gameObject.name + " (" + tag + ")");

        if (tag == "Enemy" || tag == "Corpse" || tag == "Loot" || tag == "Log")
        {
            Debug.Log("******* Add " + other.gameObject.name + " (" + tag + ")");
            //nearbyThings.Add(other.gameObject);
            //FindClosestThing();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;
        Debug.Log("****** Remove " + other.gameObject.name + " (" + tag + ")");

        if (tag == "Enemy" || tag == "Corpse" || tag == "Loot" || tag == "Log")
        {
            Debug.Log("****** Remove " + other.gameObject.name + " (" + tag + ")");
            //nearbyThings.Remove(other.gameObject);
            //FindClosestThing();
        }
    }
}
