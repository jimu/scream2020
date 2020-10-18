using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedBranch : MonoBehaviour
{
    // program branch so that it stops navigation.
    [SerializeField] private ParticleSystem dustCloud;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            dustCloud.Play();
        }
    }

    private void Awake()
    {

    }


}
