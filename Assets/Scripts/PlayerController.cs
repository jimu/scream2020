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
    [SerializeField] AudioClip sfxMovement;

    [SerializeField] InteractionIcon interactionIcon;
    [SerializeField] Sprite interactionIconTree;
    [SerializeField] bool isInteractionDirty = true;

    [SerializeField] int initialInventoryLures;
    [SerializeField] int initialInventoryTotems;
    [SerializeField] int initialInventoryBranches;
    [SerializeField] InventoryDisplay inventoryDisplay;

    GameObject areaOfEffectObject;
    int attackSoundIndex = 0;

    int inventoryLures = 0;
    int inventoryTotems = 0;
    int inventoryBranches = 0;

    // Looting
    bool isLooting = false;
    [SerializeField] float lootDuration;
    float lootFinishTime;
    GameObject enemyBeingLooted;

    List<GameObject> nearbyThings;
    GameObject closestObject = null;
    GameObject lastClosestObject = null;

    [SerializeField] UnityEngine.UI.Text inventoryText = null;


    

    
    public void SetDirty()
    {
        isInteractionDirty = true;
    }

    public AbilityBase[] abilities;

    /*
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
    */

    private void Start()
    {
        abilities = GetComponents<AbilityBase>();
        nearbyThings = new List<GameObject>();
        GameManager.instance.removeEnemyAction += RemoveNearbyThing;


        // hack - update camra's follow script (because it's don't destroy on load now)
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().Init();
       

        inventoryLures = initialInventoryLures;
        inventoryTotems = initialInventoryTotems;
        inventoryBranches = initialInventoryBranches;
        UpdateInventory();
    }


    void Update()
    {
        if (GameManager.instance.GetGameState() == GameState.Playing)
        {
            FindClosestThing();
            float deltaX = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
            float deltaZ = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;

            transform.Translate(deltaX, 0f, deltaZ);

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Button0"))
                OnInteractButtonPressed();
            if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Button3") /* || Input.GetMouseButtonDown(10) */)
                DisplayAreaOfEffect(GetComponent<DropTotemAbility>().aoe);
            if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("Button3") /* || Input.GetMouseButtonUp(0) */)
                OnTotemButtonPressed();
            if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Button2") /* || Input.GetMouseButtonDown(2) */)
                OnBranchButtonPressed();
            if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Button1") /* || Input.GetMouseButtonDown(1) */)
                DisplayAreaOfEffect(GetComponent<DropLureAbility>().aoe);
            if (Input.GetKeyUp(KeyCode.L) || Input.GetButtonUp("Button1") /* || Input.GetMouseButtonUp(1) */)
                OnLureButtonPressed();
            if (Input.GetKeyDown(KeyCode.Q))
                abilities[0].TriggerAbility();

            if (Input.GetKeyDown(KeyCode.P))
                Time.timeScale = Time.timeScale > 0 ? 0 : 0.5f;

            if (isLooting)
            {
                if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Button0"))
                    EndLooting(false);
                if (Time.time > lootFinishTime)
                {
                    GameManager.instance.PlayOneShot(sfxLootComplete);
                    EndLooting(true);
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
       // RaycastHit hit;
       // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
       if (closestObject != null)
        {
            GameObject o = closestObject;
       //     GameObject o = hit.collider.gameObject;
           // Debug.Log("Hit: " + o.name);
            if (o.CompareTag("Enemy"))
                DamageEnemyObject(o, 2);
            else if (o.CompareTag("Corpse"))
                LootCorpse(o);
            else if (o.CompareTag("Loot"))
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
        FindClosestThing();
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

    public void ClearClosestObject()
    {
        closestObject = null;
        plumbob.transform.SetParent(null, false);

    }
    int n = 0;
    private void FindClosestThing()
    {
       // Debug.Log("FindClosestThing");
        Vector3 pos = transform.position;
        pos.y = 0f;
       // string text = "INTERACTION PANEL (" + (n++) + ")\n";
        //GameManager.instance.SetInteractionText(text);

        float closestDistance = float.MaxValue;
        float highestPriority = 0;
        closestObject = null;
        Collider[] cols = Physics.OverlapSphere(pos, 4f);
       // text += cols.Length + " objects found\n";
       // GameManager.instance.SetInteractionText(text);

        foreach (Collider collider in Physics.OverlapSphere(pos, 2f)) // todo layermask
        {
            GameObject o = collider.gameObject;
            float distance = Vector3.Distance(pos, o.transform.position);
            string otag = o.tag == "Enemy" && o.GetComponent<Enemy>().GetFear() <= 0 ? "none" : o.tag;

            int priority =
                otag == "Enemy" ? 10 :
                otag == "Log" ? 8 :
                otag == "Loot" ? 6 :
                otag == "Corpse" ? 4 : -1;

            if (priority > highestPriority)
            {
                closestDistance = distance;
                closestObject = o;
                highestPriority = priority;
            }
            else if (priority == highestPriority && distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = o;
                highestPriority = priority;
            }
            //text += o.name + ":" + o.tag + " (" + priority + ")\n";
        }


      //  GameManager.instance.SetInteractionText(text);


        if (closestObject != lastClosestObject)
        {
            lastClosestObject = closestObject;
            //Transform plumbobParent = plumbob.transform.parent;

            // remove plumbob if there are no objects at all
            /*
            if (closestObject == null && plumbobParent != null)
            {
                plumbob.transform.SetParent(null, false);
                interactionIcon.gameObject.SetActive(false);
            }*/
            if (closestObject == null)
            {
                interactionIcon.gameObject.SetActive(false);
            }
            else if (closestObject != null /* &&  */)
            {
 //               if (plumbobParent != closestObject.transform)
   //                 plumbob.transform.SetParent(closestObject.transform, false);

                GameManager.instance.PlayOneShotIfGamePlay(sfxSnapToTarget);
                //Debug.Log("Selected: " + closestObject.name);
                interactionIcon.Follow(closestObject.transform, interactionIconTree);
                interactionIcon.gameObject.SetActive(true);
            }
     //       plumbob.SetActive(closestObject != false);
        }
    }


    // method - rename closestEnemies to deadzone, defined by player's collider

    // Deadzone for attack.  Uses closest enemies array (never should have optimized this)
    // closest enemies uses collider.  need to prioritieze functions - kill over loot

    // out: member property closestThing
    private void FindClosestThing0()
    {



        Vector3 pos = transform.position;
        float closestDistance = float.MaxValue;
        float highestPriority = int.MinValue;
        closestObject = null;

        string text = "INTERACITON PANEL\n";

        foreach (GameObject o in nearbyThings)
        {
            if (o != null)
            {
                float distance = Vector3.Distance(pos, o.transform.position);
                int priority =
                    tag == "Enemy" ? 10 :
                    tag == "Log" ? 8 :
                    tag == "Loot" ? 6 :
                    tag == "Corpse" ? 4 : 2;

                if (priority > highestPriority)
                {
                    closestDistance = distance;
                    closestObject = o;
                    highestPriority = priority;
                }
                else if (priority == highestPriority && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = o;
                    highestPriority = priority;
                }
                text = o.name + ":" + o.tag + "\n";
            }
            else
                text = "NULL\n";
        }

        Transform plumbobParent = plumbob.transform.parent;

        // remove plumbob
        if (closestObject == null && plumbobParent != null)
        {
            plumbob.transform.SetParent(null, false);
            interactionIcon.gameObject.SetActive(false);
        }

        else if (closestObject != null && plumbobParent != closestObject.transform)
        {
            plumbob.transform.SetParent(closestObject.transform, false);
            GameManager.instance.PlayOneShotIfGamePlay(sfxSnapToTarget);
            //Debug.Log("Selected: " + closestObject.name);
            interactionIcon.Follow(closestObject.transform, interactionIconTree);
            interactionIcon.gameObject.SetActive(true);
        }



        plumbob.SetActive(closestObject != false);
        GameManager.instance.SetInteractionText(text);
    }


    //#####################################################################################
    // compile everything here.  prioritize contents when interacting

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if ((tag == "Enemy" && other.gameObject.GetComponent<Enemy>().GetFear() > 0) || tag == "Corpse" || tag == "Loot" || tag == "Log")
        {
            //Debug.Log("Add " + other.gameObject.name + " (" + tag + ")");
            nearbyThings.Add(other.gameObject);
            FindClosestThing();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "Enemy" || tag == "Corpse" || tag == "Loot" || tag == "Log")
        {
            //Debug.Log("Remove " + other.gameObject.name + " (" + tag + ")");
            nearbyThings.Remove(other.gameObject);
            FindClosestThing();
        }
    }

    public void RemoveThing(GameObject o)
    {
        nearbyThings.Remove(o);

    }

    void RemoveNearbyThing(Enemy enemy)
    {
        nearbyThings.Remove(enemy.gameObject);
    }


    public void OnInteractButtonPressed()
    {
       // Debug.Log("Interact Pressed!");
        Interact();
    }


    public void OnBranchButtonPressed()
    {
        if (inventoryBranches > 0)
        {
            //Debug.Log("Branch Pressed!");
            GetComponent<DropBranchAbility>().TriggerAbility();
            inventoryBranches--;
            UpdateInventory();
            GameManager.instance.SetDirty();
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
            //Debug.Log("Totem Pressed!");
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

    public void EndLooting(bool finished)
    {
        isLooting = false;
        GameManager.instance.StopProgressBar();

        if (finished)
        {
            inventoryBranches++;
            inventoryLures++;
            inventoryTotems++;
            GameManager.instance.SetDirty();
        }
        UpdateInventory();
    }


    public void ResetInteractionIcon()
    {
        interactionIcon.gameObject.SetActive(false);
        plumbob.transform.parent = null;
        plumbob.SetActive(false);
        FindClosestThing();
    }

    void UpdateInventory()
    {
        inventoryDisplay.UpdateValues(inventoryLures, inventoryBranches, inventoryTotems);
    }

}

