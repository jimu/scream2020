using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrorMeter : MonoBehaviour
{
    [SerializeField] GameObject[] icons;
    [SerializeField] Sprite goodSprite;
    [SerializeField] Sprite badSprite;
    int maxIcons = 4;

    void Start()
    {
        for (int i = 0; i < maxIcons; i++)
        {/*
            Vector3 v = new Vector3(-178f + i * 45, -50f, 0f);
            
            goodIcons[i] = Instantiate(goodPrefab, v, Quaternion.identity);
            badIcons[i] = Instantiate(badPrefab, v, Quaternion.identity);
            goodIcons[i].transform.parent = gameObject.transform;
            float x = gameObject.transform.position.x;
            Debug.Log(gameObject.transform.position.x);
            Debug.Log(x);
            SpriteRenderer sr = goodIcons[i].GetComponent<SpriteRenderer>();
            Debug.Log(sr.sprite);
            */
        }
        Set(2);
    }

    // Update is called once per frame
    void Set(int n)
    {
        for (int i = 0; i < maxIcons; ++i)
            icons[i].GetComponent<SpriteRenderer>().sprite = n < i ? badSprite : goodSprite;

    }
}
