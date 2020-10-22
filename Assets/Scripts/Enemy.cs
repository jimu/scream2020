using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0414,0649

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxHurt;
    [SerializeField] AudioClip sfxEatLure;
    [SerializeField] public int value = 1;
    [SerializeField] public int health = 1;
    [SerializeField] float speed = 3.5f;
    Animator animator = null;
    //NavMeshAgent navMeshAgent;
    MoveTo moveTo;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        moveTo = GetComponent<MoveTo>();
    }

    private void Start()
    {
        moveTo.SetSpeed(speed);
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
            moveTo.SetSpeed(0f);
            StartCoroutine(WaitAndDestroy(other.gameObject, 3f)); // 3 seconds
        }
        else if (other.gameObject.CompareTag("Exit"))
        {
            Debug.Log("Enemy escaped:" + gameObject.name);
            GameManager.instance.RemoveEnemy(this);
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Lure"))
        {
            StartCoroutine(WaitEatLureAndResume(other.gameObject, 10f, other.gameObject.GetComponent<Lure>().GetId()));
        }
    }

    private IEnumerator WaitAndDestroy(GameObject gameObject, float delay)
    {
        string name = gameObject.name;
        Debug.Log("WaitAndDestroy:begin(" + name + ")");
        yield return new WaitForSeconds(delay);
        Debug.Log("WaitAndDestroy:begin(" + name + ")");
        if (gameObject != null)
            Destroy(gameObject);
        moveTo.SetSpeed(speed);
    }

    private IEnumerator WaitEatLureAndResume(GameObject lureObject, float delay, int id)
    {
        string name = lureObject.name;

        Debug.Log("WaitAndDestroy:begin(" + name + ")");
        yield return new WaitForSeconds(delay);
        Debug.Log("WaitAndDestroy:end(" + name + ")");

        GameManager.instance.PlayOneShot(sfxEatLure);
        if (gameObject != null)
        {
            lureObject.GetComponent<Lure>().LureExpiredRepath();
            Destroy(lureObject);
        }
    }


    public void Die()
    {
        GameManager.instance.AddScore(value);
        GetComponent<NavMeshAgent>().enabled = false;
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxDeath);
        gameObject.tag = "Corpse";
        GameManager.instance.RemoveEnemy(this);

        if (animator == null)
        {   // cylinder
            float rotation = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(90, rotation, 0);
            transform.Translate(0, -1f, 0, Space.World);
        }
        else
        {
            animator.SetBool("Dead", true);
            gameObject.GetComponent<CapsuleCollider>().radius = 2.0f;
        }
    }

    public void RecalculateNavigation()
    {
        gameObject.GetComponent<MoveTo>().RecalculateNavigation();
    }


    /** Go to lure if in range */
    public void LureCheck(int id, Vector3 lurePosition, float range)
    {
        //Debug.Log("Enemy " + name + ": Checking for lure " + id);

        if (Vector3.Distance(transform.position, lurePosition) <= range)
        {
            Debug.Log("LURE PLACE-ATTRACT: " + name);
            moveTo.SetLure(id, lurePosition);
        }
    }

    public void LureRelease(int id)
    {
        moveTo.CancelLure(id, speed);
    }



}

