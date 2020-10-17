using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
public class PlayerController : MonoBehaviour
{
    float playerSpeed = 10.0f;

    [SerializeField] GameObject clawMarkPrefab;
    [SerializeField] GameObject branchObstaclePrefab;
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
            Attack();
        if (Input.GetKeyDown(KeyCode.Q))
            abilities[0].TriggerAbility();
    }

    void Attack()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && hit.collider.gameObject.CompareTag("Enemy"))
        {
            GameObject o = hit.collider.gameObject;
            Enemy enemy = o.GetComponent<Enemy>();
            enemy.Damage(1);
        }
        else
        {
            Vector3 markPos = transform.position;
            markPos.y = 0.01f;
            Instantiate(clawMarkPrefab, markPos, Quaternion.Euler(90, 0, 0));
        }
    }

}
