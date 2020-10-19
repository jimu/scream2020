using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DropTotemAbility : AbilityBase
{

    [Header("Connections")]
    [SerializeField] private GameObject totemPrefab;
    [SerializeField] private Animator animator = default;
    [SerializeField] private ParticleSystem debris;
    [SerializeField] AudioClip sfx;
    [SerializeField] float totemDuration = 15f;
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
            NavMeshObstacle nmo = totem.AddComponent(typeof(NavMeshObstacle)) as NavMeshObstacle;
            Debug.Log("nmo:" + nmo.name);
            nmo.shape = NavMeshObstacleShape.Capsule;
            nmo.radius = 4.0f;
            nmo.carving = true;

            StartCoroutine(DestroyTotem(totem));

            GameManager.instance.PlayOneShot(sfx);
            return;
        }
    }

    private IEnumerator DestroyTotem(GameObject totem)
    {
        Debug.Log("DestroyTotem: " + totem.name);
        yield return new WaitForSeconds(totemDuration);
        Debug.Log("DestroyTotem2: " + totem.name);
        totem.GetComponent<NavMeshObstacle>().enabled = false;
        Destroy(totem);
        GameManager.instance.RecalculateNavigation();
    }


}
