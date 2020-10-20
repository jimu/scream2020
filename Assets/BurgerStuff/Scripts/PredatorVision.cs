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
        Debug.Log("PredatorVision: " + (isEnabled ? "ON" : "OFF"));
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Enemy"))
            gameObject.GetComponent<SkinnedMeshRenderer>().enabled = isEnabled;
    }
}
