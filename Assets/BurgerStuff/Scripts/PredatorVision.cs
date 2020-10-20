using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{
    public bool isEnabled = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            Toggle();
    }

    public void Toggle()
    {
        isEnabled = !isEnabled;
        Debug.Log("PredatorVision: " + (isEnabled ? "ON" : "OFF") + "(" + gameObject.name + ")");
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Enemy"))
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = isEnabled;
    }
}
