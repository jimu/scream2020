using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBranchAbility : AbilityBase
{

    [Header("Connections")]
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private Animator animator = default;
    [SerializeField] private ParticleSystem debris;
    //Audio System?
    public override void Ability()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            return;
        }
        else
        {
            animator.SetTrigger("Drop Branch");
            Instantiate(branchPrefab);
            //play audio system
        }



    }


}
