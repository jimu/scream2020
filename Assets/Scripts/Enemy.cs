using System;
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
    [SerializeField] int fear = 1;
    [SerializeField] Transform fearPrefab;

    [SerializeField] bool isWandering = true;

    [SerializeField] float minWanderTime = 5.0f;
    [SerializeField] float maxWanderTime = 30.0f;
    [SerializeField] float wanderPhaseDuration = 60.0f;

    float wanderExpireTime;
    Vector3 wanderDestination;

    [Tooltip("All enemies within this distance from a kill are scared")]
    [SerializeField] float fearDistanceFromKill = 15;
    FearIcon fearIcon;


    Animator animator = null;
    //NavMeshAgent navMeshAgent;
    MoveTo moveTo;

    public event Action<float> OnFearChanged = delegate { };


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        moveTo = GetComponent<MoveTo>();
    }

    private void Start()
    {
        moveTo.SetSpeed(speed);
        fearIcon = Instantiate(fearPrefab).GetComponent<FearIcon>();
        SetFear(fear);
        if (isWandering)
            SetRandomWanderTimeAndDestination();
    }

    void SetRandomWanderTimeAndDestination()
    {
        wanderExpireTime = Time.time + UnityEngine.Random.Range(minWanderTime, maxWanderTime);
        wanderDestination = GameManager.instance.getRandomWanderPoint();
        moveTo.SetWander(wanderDestination);
    }

    public static void CancelAllWandering()
    {
        foreach (Enemy enemy in GameManager.instance.enemies)
            enemy.CancelWandering();
    }


    void CancelWandering()
    {
        isWandering = false;
        moveTo.CancelWander();
    }

    public int GetFear()
    {
        return fear;
    }

    private void Update()
    {
        if (GameManager.instance.GetGameState() == GameState.Playing && fearIcon != null)
            fearIcon.MoveTo(transform.position);
        if (isWandering && Time.time > wanderExpireTime)
            SetRandomWanderTimeAndDestination();
    }

    void SetFear(int n)
    {
        fear = n;
        fearIcon.SetFear(fear);
        moveTo.SetSpeed(speed + fear * 0.2f);

    }

    public void AddFear(int n)
    {
        SetFear(fear + n);
        CancelWandering();
    }

    public void Damage(int hits)
    {
        if (health > 0 && fear > 0)
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
            DestroyMe();
        }
        else if (other.gameObject.CompareTag("Lure"))
            StartCoroutine(WaitEatLureAndResume(other.gameObject, 10f, other.gameObject.GetComponent<Lure>().GetId()));
        else if (other.gameObject.CompareTag("Totem"))
            AddFear(1);
        else if (other.gameObject.CompareTag("WanderPoint") && other.gameObject.transform.position == wanderDestination)
            SetRandomWanderTimeAndDestination();
    }

    public void DestroyMe()
    {
        if (this.fearIcon != null)
            Destroy(this.fearIcon.gameObject);
        Destroy(this.gameObject);
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
        //Debug.Log("Score: value " + value + "  X  fear " + fear);
        GameManager.instance.AddScore(value * fear);
        GetComponent<NavMeshAgent>().enabled = false;
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxDeath);
        gameObject.tag = "Corpse";
        GameManager.instance.RemoveEnemy(this);
        fearIcon.gameObject.SetActive(false);
        Destroy(fearIcon);
        GameManager.instance.ResetInteractionIcon();
        // Debug.Log("RESET II");
        GameManager.instance.SetDirty();


        if (animator == null)
        {   // cylinder
            float rotation = UnityEngine.Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(90, rotation, 0);
            transform.Translate(0, -1f, 0, Space.World);
        }
        else
        {
            animator.SetBool("Dead", true);
            gameObject.GetComponent<CapsuleCollider>().radius = 2.0f;
        }

        ScareNearbyEnemies(transform.position, fearDistanceFromKill);
    }
    
    static public void ScareNearbyEnemies(Vector3 pos, float distance)
    {
        Debug.Log("ScareNearbyEnemies(" + pos + ", " + distance + ")");
        pos.y = 1f;
        foreach (Enemy enemy in GameManager.instance.enemies)
            if (Vector3.Distance(enemy.transform.position, pos) < distance)
                enemy.AddFear(1);
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

