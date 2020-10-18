﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 20.0f;

    [SerializeField] GameObject clawMarkPrefab;
    [SerializeField] GameObject plumbob;
    [SerializeField] GameObject branchObstaclePrefab;
    [SerializeField] AudioClip sfxSnapToTarget;
    [SerializeField] AudioClip[] attackSounds;
    int attackSoundIndex = 0;
    List<GameObject> nearbyEnemies;
    GameObject closestObject = null;

    public AbilityBase[] abilities;


    private void Start()
    {
        abilities = GetComponents<AbilityBase>();
        nearbyEnemies = new List<GameObject>();
    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
        float deltaZ = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
        transform.Translate(deltaX, 0f, deltaZ);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
            Interact();
        if (Input.GetKeyDown(KeyCode.Q))
            abilities[0].TriggerAbility();
    }
    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            GameObject o = hit.collider.gameObject;
            if (o.CompareTag("Enemy"))
                DamageEnemyObject(o, 2);
            else if (o.CompareTag("Branch"))
                o.GetComponent<Branch>().Block();
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
            GameManager.instance.PlayOneShot(sfxSnapToTarget);
            Debug.Log("Selected: " + closestObject.name);
        }

        plumbob.SetActive(closestObject != false);

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Adding " + other.gameObject.name);
        if (other.gameObject.CompareTag("Enemy"))
            nearbyEnemies.Add(other.gameObject);
        FindClosestEnemy();
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Remove " + other.gameObject.name);
        GameObject o = other.gameObject;
        if (o.CompareTag("Enemy"))
            nearbyEnemies.Remove(o);
        FindClosestEnemy();
    }

}
