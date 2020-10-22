using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0649,0414
public class DropTotemAbility : AbilityBase
{

    [Header("Connections")]
    [SerializeField] private GameObject totemPrefab;
    [SerializeField] private Animator animator = default;
    [SerializeField] private ParticleSystem debris;
    [SerializeField] AudioClip sfx;
    [SerializeField] float totemDuration = 15f;
    [SerializeField] public GameObject aoe;
    [SerializeField] bool isBlocking = false;
    

    [SerializeField] bool causesFearOnApproach = true;

    [SerializeField] bool causesFearOnDrop = true;
    
    [SerializeField] float effectRadius = 4.0f;
    [SerializeField] float effectRadiusOnDrop = 4.0f;
    //Audio System?
    public override void Ability()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //animator.SetTrigger("Drop Branch");
            Vector3 pos = hit.point;
            pos.y = 0f;
            GameObject totem = Instantiate(totemPrefab, pos, Quaternion.identity);
            /*
            SphereCollider sc = totem.AddComponent(typeof(SphereCollider)) as SphereCollider;
            sc.radius = 4;
            sc.isTrigger = true;
            totem.tag = "Totem";
            NavMeshObstacle nmo = totem.AddComponent(typeof(NavMeshObstacle)) as NavMeshObstacle;
            nmo.size = 
            */
            if (isBlocking)
            {
                NavMeshObstacle nmo = totem.AddComponent(typeof(NavMeshObstacle)) as NavMeshObstacle;

                nmo.shape = NavMeshObstacleShape.Capsule;
                nmo.radius = effectRadius;
                nmo.carving = true;
            }
            if (causesFearOnDrop)
            {
                foreach(Enemy enemy in GameManager.instance.enemies)
                    if (Vector3.Distance(enemy.transform.position, transform.position) < effectRadiusOnDrop)
                        enemy.AddFear(1);
            }

            if (causesFearOnApproach)
            {
                SphereCollider sphereCollider = totem.AddComponent(typeof(SphereCollider)) as SphereCollider;

                sphereCollider.radius = effectRadius;
            }

            StartCoroutine(DestroyTotem(totem));

            GameManager.instance.PlayOneShot(sfx);
            return;
        }
    }

    private IEnumerator DestroyTotem(GameObject totem)
    {
        //Debug.Log("DestroyTotem: " + totem.name);
        yield return new WaitForSeconds(totemDuration);
        
        NavMeshObstacle nmo = totem.GetComponent<NavMeshObstacle>();
        
        if (nmo)
            nmo.enabled = false;
        
        Destroy(totem);
        GameManager.instance.RecalculateNavigation();
    }


}
