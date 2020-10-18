using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public abstract class PredatorVision : AbilityBase
{

    [SerializeField] private SkinnedMeshRenderer predMr;
    [SerializeField] private bool predVisionActive;
    private void Start()
    {
        predMr = GetComponent<SkinnedMeshRenderer>();
    }
    public override void Ability()
    {
        if (predVisionActive != true)
        {
            RaycastHit[] castArray = new RaycastHit[12];
            RaycastHit[] array = Physics.RaycastAll(transform.position, transform.forward,100);


            for (int hitIndex = 0; hitIndex < array.Length; ++hitIndex)
            {
                RaycastHit hit = castArray[hitIndex];
                SkinnedMeshRenderer[] childrenRenderers = hit.collider.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int rendererIndex = 0; rendererIndex < childrenRenderers.Length; ++rendererIndex)
                {
                    childrenRenderers[rendererIndex].enabled = false;
                }
            }


            //for (int hitIndex = 0; hitIndex < array.length; ++hitIndex)
            //{
            //    RaycastHit hit = array[hitIndex];
            //    MeshRenderer[] childrenRenderers = hit.collider.gameObject.GetComponentsInChildren<MeshRenderer>();
            //    for (int rendererIndex; rendererIndex < childrenRenderers.Length; ++rendererIndex)
            //        childrenRenderers[rendererIndex].enabled = false;
            //}

        }

    }
}







