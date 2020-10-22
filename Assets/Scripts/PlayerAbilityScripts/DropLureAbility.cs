using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0649,0414
public class DropLureAbility : AbilityBase
{

    [Header("Connections")]
    [SerializeField] private GameObject lurePrefab;
    [SerializeField] private Animator animator = default;
    [SerializeField] private ParticleSystem debris;
    [SerializeField] public GameObject aoe;
    [SerializeField] AudioClip sfx;
    // [SerializeField] float lureDuration = 15f;   // does not decay
    //Audio System?
    public override void Ability()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //animator.SetTrigger("Drop Branch");
            Vector3 pos = hit.point;
            pos.y = 0f;
            Instantiate(lurePrefab, pos, Quaternion.identity);
            GameManager.instance.PlayOneShot(sfx);
            return;
        }
    }
}
