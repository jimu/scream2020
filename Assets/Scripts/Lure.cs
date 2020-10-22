using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : MonoBehaviour
{
    
    
    static int lastId = -1;
    int id;
    [Header("Area of effect of lure")]
    [Tooltip("Area of effect of lure")]
    [SerializeField] float range = 10f;

    [Header("Number of seconds enemy is delayed")]
    [Tooltip("Number of seconds enemy is delayed")]
    [SerializeField] float delay = 1f;


    public int GetId()
    {
        return id;
    }

    // Start is called before the first frame update
    void Awake()
    {
        id = ++lastId;
        Debug.Log("Lure Created: " + id);
        transform.localScale = new Vector3(range, range, range);
        Check();
    }

    public void Check()
    {
        Debug.Log("Lure " + id + ": Checking for enemies within " + range);
        foreach (Enemy enemy in GameManager.instance.enemies)
            enemy.LureCheck(id, transform.position, range);

    }

    // Update is called once per frame
    public void LureExpiredRepath()
    {
        Debug.Log("Lure " + id + ": Expires. Releasing all enemies");
        foreach (Enemy enemy in GameManager.instance.enemies)
            enemy.LureRelease(id);
    }
}
