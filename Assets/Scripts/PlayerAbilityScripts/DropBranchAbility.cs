using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBranchAbility : AbilityBase
{

    [Header("Connections")]
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private Animator animator = default;
    [SerializeField] private ParticleSystem debris;
    [SerializeField] AudioClip sfx;
    //Audio System?
    public override void Ability()
    { 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //animator.SetTrigger("Drop Branch");
            Vector3 pos = hit.point;
            pos.y = 0f;
            Instantiate(branchPrefab, pos, Quaternion.identity);

            GameManager.instance.PlayOneShot(sfx);
            //play audio system
            return;
        }
        else
        {
        }



    }


}
