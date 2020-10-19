using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0414,0649

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxHurt;
    [SerializeField] public int value = 1;
    [SerializeField] public int health = 1;
    [SerializeField] float speed = 3.5f;
    Animator animator = null;
    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Damage(int hits)
    {
        if (health > 0)
        {
            health -= hits;
            if (health <= 0)
                Die();
            else
                GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxHurt);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Branch"))
        {
            navMeshAgent.speed = 0f;
            StartCoroutine(WaitAndDestroy(other.gameObject));
        }
    }

    private IEnumerator WaitAndDestroy(GameObject gameObject)
    {
        Debug.Log("WaitAndDestroy:begin");
        yield return new WaitForSeconds(3f);
        Debug.Log("WaitAndDestroy:end");
        Destroy(gameObject);
        navMeshAgent.speed = speed;
    }


    public void Die()
    {
        GameManager.instance.AddScore(value);
        GetComponent<NavMeshAgent>().enabled = false;
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxDeath);

        if (animator == null)
        {   // cylinder
            float rotation = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(90, rotation, 0);
            transform.Translate(0, -1f, 0, Space.World);
        }
        else
        {
            animator.SetBool("Dead", true);
        }
    }

    public void RecalculateNavigation()
    {
        gameObject.GetComponent<MoveTo>().RecalculateNavigation();
    }
}

