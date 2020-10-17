using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
public class PlayerController : MonoBehaviour
{
    float playerSpeed = 10.0f;

    [SerializeField] GameObject clawMarkPrefab;
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

        if (Input.GetKeyDown(KeyCode.Space))
            Attack();
        if (Input.GetKeyDown(KeyCode.Q))
            abilities[0].TriggerAbility();
    }

    void Attack()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            Debug.Log("hit: " + hit.collider.gameObject.name);
            Destroy(hit.collider.gameObject);
        }
        else
        {
            Vector3 markPos = transform.position;
            markPos.y = 0.01f;
            Instantiate(clawMarkPrefab, markPos, Quaternion.Euler(90, 0, 0));
        }
    }

}
