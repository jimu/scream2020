using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 20.0f;

    [SerializeField] GameObject clawMarkPrefab;
    [SerializeField] GameObject branchObstaclePrefab;
    [SerializeField] AudioClip[] attackSounds;
    int attackSoundIndex = 0;
    public AbilityBase[] abilities;

    private void Start()
    {
        abilities = GetComponents<AbilityBase>();
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
                o.GetComponent<Enemy>().Damage(1);
            else if (o.CompareTag("Branch"))
                o.GetComponent<Branch>().Block();
            else
                AttackMissed();
        }
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

}
