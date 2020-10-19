using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{


    public bool predVisionActive;
    public GameManager gameManager;



    public void Toggle()
    {
        Debug.Log("Toggle");
        if (predVisionActive)
        {
            Deactivate();
            predVisionActive = false;
        }
        else
        {
            Activate();
        }
    }
    void Activate()
    {

        foreach (Enemy enemy in gameManager.enemies)
        {
            enemy.predVisionActive = true;
            Debug.Log("Activate");
            predVisionActive = true;

        }

    }
    void Deactivate()
    {

        foreach (Enemy enemy in gameManager.enemies)
        {
            Debug.Log("Deactivate");
            enemy.predVisionActive = false;
            predVisionActive = false;

        }

    }

}


//if (predVisionActive != true)
//{




//    RaycastHit[] castArray = new RaycastHit[12];
//    RaycastHit[] array = Physics.RaycastAll(transform.position, transform.forward, 100);


//    for (int hitIndex = 0; hitIndex < array.Length; ++hitIndex)
//    {
//        RaycastHit hit = castArray[hitIndex];
//        SkinnedMeshRenderer[] childrenRenderers = hit.collider.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

//        for (int rendererIndex = 0; rendererIndex < childrenRenderers.Length; ++rendererIndex)
//        {
//            childrenRenderers[rendererIndex].enabled = false;
//        }
//    }


//    for (int hitIndex = 0; hitIndex < array.length; ++hitIndex)
//    {
//        RaycastHit hit = array[hitIndex];
//        MeshRenderer[] childrenRenderers = hit.collider.gameObject.GetComponentsInChildren<MeshRenderer>();
//        for (int rendererIndex; rendererIndex < childrenRenderers.Length; ++rendererIndex)
//            childrenRenderers[rendererIndex].enabled = false;
//    }

//}





