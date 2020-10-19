using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer2 : MonoBehaviour
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
