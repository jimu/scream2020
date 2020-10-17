using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatSetter : MonoBehaviour
{
    public AudioPeer audiopeer;
    public Material shaderMat;
    private float intensity = 0;
    public float minimum = 0;
    public float maximum = 1;


    private void Start()
    {

        shaderMat.SetFloat("Vector1_9D18033A", intensity * Time.time);
    }


    // uses the spectrum data somehow to  math sound into LIGHT! Updates the vector1 slider present  on the shader material present in the editor.
    private void Update()
    {
        float sum = 0;
        float max = 0;
        for (int i = 0; i < 512; i++)
        {
            max = Mathf.Max(max, audiopeer.samples[i]);
            sum += audiopeer.samples[i];
        }

        Debug.Log(sum);
        shaderMat.SetFloat("Vector1_9D18033A", sum);


    }

}

