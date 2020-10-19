using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat2 : MonoBehaviour
{
    public AudioPeer2 audiopeer;
    public Material shaderMat;
    private float intensity = 0;
    public float minimum = 0;
    public float maximum = 1;


    private void Start()
    {
        shaderMat.SetFloat("Vector1_9D18033A", intensity * Time.time);
    }


    //    float GetIntensity()
    //    {
    //        float[] intensity = [0.0, 0.05, 0.07, 0.11, 0.25, 0.30, 0.50, 0.65, 0.70, 0.95,1 ];  // values over one beat
    //        float t = Mathf.Floor(Time.time * heartRate) % intensity.Length;
    //        return intensity[index];
    //    }

    private void Update()
    {
        float sum = 0;
        float max = 0;
        for (var i = 0; i < 512; i++)
        {
            max = Mathf.Max(max, audiopeer.samples[i]);
            sum += audiopeer.samples[i];
        }

        Debug.Log(sum);
        shaderMat.SetFloat("Vector1_9D18033A", sum);


    }

}
