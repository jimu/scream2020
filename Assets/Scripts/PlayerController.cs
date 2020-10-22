using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0649
public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 20.0f;

    [SerializeField] GameObject clawMarkPrefab;
    [SerializeField] GameObject plumbob;
    [SerializeField] GameObject logObstaclePrefab;
    [SerializeField] AudioClip sfxSnapToTarget;
    [SerializeField] AudioClip sfxLooting;
    [SerializeField] AudioClip sfxLootComplete;
    [SerializeField] AudioClip[] attackSounds;
    
    GameObject areaOfEffectObject;
    int attackSoundIndex = 0;

    int inventoryLures = 5;
    int inventoryTotems = 5;
    int inventoryBranches = 5;

    // Looting
    bool isLooting = false;
    [SerializeField] float lootDuration;
    float lootFinishTime;
    GameObject enemyBeingLooted;

    List<GameObject> nearbyEnemies;
    GameObject closestObject = null;

    [SerializeField] UnityEngine.UI.Text inventoryText = null;




    public AbilityBase[] abilities;


    private void UpdateInventory()
    {
        string message = "";
        if (inventoryTotems > 0)
            message += "Totems: " + inventoryTotems + "\n";
        if (inventoryLures > 0)
            message += "Lures: " + inventoryLures + "\n";
        if (inventoryBranches > 0)
            message += "Branches: " + inventoryBranches + "\n";

        if (inventoryText != null)
            inventoryText.text = message;
        else
            Debug.Log(message);
    }


    private void Start()
    {
        abilities = GetComponents<AbilityBase>();
        nearbyEnemies = new List<GameObject>();
        GameManager.instance.removeEnemyAction += RemoveNearbyEnemy;
        UpdateInventory();

        // hack - update camra's follow script (because it's don't destroy on load now)
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().Init();
    }


    void Update()
    {
        if (GameManager.instance.GetGameState() == GameState.Playing)
        {
            float deltaX = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
            float deltaZ = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
            transform.Translate(deltaX, 0f, deltaZ);

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Button0"))
                OnInteractButtonPressed();
            if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Button1") || Input.GetMouseButtonDown(0))
                DisplayAreaOfEffect(GetComponent<DropTotemAbility>().aoe);
            if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("Button1") || Input.GetMouseButtonUp(0))
                OnTotemButtonPressed();
            if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Button2") || Input.GetMouseButtonDown(2))
                OnBranchButtonPressed();
            if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Button3") || Input.GetMouseButtonDown(1))
                DisplayAreaOfEffect(GetComponent<DropLureAbility>().aoe);
            if (Input.GetKeyUp(KeyCode.L) || Input.GetButtonUp("Button3") || Input.GetMouseButtonUp(1))
                OnLureButtonPressed();
            if (Input.GetKeyDown(KeyCode.Q))
                abilities[0].TriggerAbility();

            if (isLooting)
            {
                if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Button0"))
                    EndLooting();
                if (Time.time > lootFinishTime)
                {
                    GameManager.instance.PlayOneShot(sfxLootComplete);
                    EndLooting();
                    Debug.Log("Got a totem");
                    Destroy(enemyBeingLooted);
                    enemyBeingLooted = null;
                }
            }
        }

    }

    void HideAreaOfEffect()
    {
        if (areaOfEffectObject)
        {
            areaOfEffectObject.SetActive(false);
        }
    }

    void DisplayAreaOfEffect(GameObject aoe)
    {
        areaOfEffectObject = aoe;
        areaOfEffectObject.SetActive(true);
    }


    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            GameObject o = hit.collider.gameObject;
            Debug.Log("Hit: " + o.name);
            if (o.CompareTag("Enemy"))
                DamageEnemyObject(o, 2);
            else if (o.CompareTag("Corpse")) 
                LootCorpse(o);
            else if (o.CompareTag("Log"))
                o.GetComponent<Log>().Block();
            else if (closestObject != null)
                DamageEnemyObject(closestObject, 1);
            else
                AttackMissed();
        }
    }

    void DamageEnemyObject(GameObject enemyObject, int hits)
    {

        enemyObject.GetComponent<Enemy>()?.Damage(hits);
        FindClosestEnemy();
    }

    void AttackMissed()
    {
        Vector3 markPos = transform.position;
        markPos.y = 0.01f;
        Instantiate(clawMarkPrefab, markPos, Quaternion.Euler(90, 0, 0));
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(attackSounds[attackSoundIndex]);
        attackSoundIndex = (attackSoundIndex + 1) % attackSounds.Length;
    }
    void AttackDepricated(Enemy enemy)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && hit.collider.gameObject.CompareTag("Enemy"))
        {
            GameObject o = hit.collider.gameObject;
            Enemy enemyx = o.GetComponent<Enemy>();
            enemy.Damage(1);
        }
        else
        {
            Vector3 markPos = transform.position;
            markPos.y = 0.01f;
            Instantiate(clawMarkPrefab, markPos, Quaternion.Euler(90, 0, 0));
            GameManager.instance.GetComponent<AudioSource>().PlayOneShot(attackSounds[attackSoundIndex]);
            attackSoundIndex = (attackSoundIndex + 1) % attackSounds.Length;
        }
    }

    private void FindClosestEnemy()
    {
        Vector3 pos = transform.position;
        float closestDistance = float.MaxValue;
        closestObject = null;
        foreach (GameObject o in nearbyEnemies)
        {
            float distance = Vector3.Distance(pos, o.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = o;
            }
        }

        Transform parent = plumbob.transform.parent;

        if (closestObject == null && parent != null)
            plumbob.transform.SetParent(null, false);

        if (closestObject != null && parent != closestObject.transform)
        {
            plumbob.transform.SetParent(closestObject.transform, false);
            GameManager.instance.PlayOneShotIfGamePlay(sfxSnapToTarget);
            //Debug.Log("Selected: " + closestObject.name);
        }

        plumbob.SetActive(closestObject != false);

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Adding " + other.gameObject.name);
        if (other.gameObject.CompareTag("Enemy"))
            nearbyEnemies.Add(other.gameObject);
        FindClosestEnemy();
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Remove " + other.gameObject.name);
        GameObject o = other.gameObject;
        if (o.CompareTag("Enemy"))
            nearbyEnemies.Remove(o);
        FindClosestEnemy();
    }

    void RemoveNearbyEnemy(Enemy enemy)
    {
        nearbyEnemies.Remove(enemy.gameObject);
    }


    public void OnInteractButtonPressed()
    {
        Debug.Log("Interact Pressed!");
        Interact();
    }


    public void OnBranchButtonPressed()
    {
        if (inventoryBranches > 0)
        {
            Debug.Log("Branch Pressed!");
            GetComponent<DropBranchAbility>().TriggerAbility();
            inventoryBranches--;
            UpdateInventory();
        }
        else
        {
            // fail sound - branch 
            Debug.Log("No Branches in inventory");
        }
    }

    public void OnTotemButtonPressed()
    {
        HideAreaOfEffect();
        if (inventoryTotems > 0)
        {
            Debug.Log("Totem Pressed!");
            GetComponent<DropTotemAbility>().TriggerAbility();
            inventoryTotems--;
            UpdateInventory();
        }
        else
        {
            // fail sound - totem
            Debug.Log("No Totems in inventory");
        }
    }

    public void OnLureButtonPressed()
    {
        HideAreaOfEffect();
        if (inventoryLures > 0)
        {
            Debug.Log("Lure Pressed!");
            GetComponent<DropLureAbility>().TriggerAbility();
            inventoryLures--;
            UpdateInventory();

        }
        else
        {
            // fail sound - lure
            Debug.Log("No Lures in inventory");
        }
    }


    public void LootCorpse(GameObject o)
    {
        Debug.Log("LootCorpse");
        isLooting = true;
        GameManager.instance.PlayOneShot(sfxLooting);
        enemyBeingLooted = o;
        lootFinishTime = Time.time + lootDuration;
        GameManager.instance.StartProgressBar(lootDuration);

    }

    public void EndLooting()
    {
        isLooting = false;
        GameManager.instance.StopProgressBar();

        inventoryBranches++;
        inventoryLures++;
        inventoryTotems++;
        UpdateInventory();
    }
}
