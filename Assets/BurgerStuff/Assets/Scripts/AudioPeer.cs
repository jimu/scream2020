using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof (AudioSource))]

//Audiopeer takes the spectrum data from the audiosource loaded on the AudioPeer game object and creates 512 samples from that data.
public class AudioPeer : MonoBehaviour
{
    AudioSource audioSource;
    public float[] samples = new float[512];
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        GetSpectrumAudioSource();
    }
    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

}
