using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVisionAbility : AbilityBase
{
    [SerializeField] private float abilityRange = 100;
    public override void Ability()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, abilityRange, Vector3.up);
        for(int i=0;i < hits.Length; i++)
        {
            if(hits[i].collider.gameObject == gameObject)
            {
                continue;
            }
            if (hits[i].collider.CompareTag("Enemy"))
            {
                GetComponent<Enemy>();
            }
        }

    }
}
