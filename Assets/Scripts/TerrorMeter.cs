using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

public class TerrorMeter : MonoBehaviour
{
    [SerializeField] GameObject[] icons;
    [SerializeField] Sprite goodSprite;
    [SerializeField] Sprite badSprite;
    
    void Start()
    {
        Set(0);
    }

    // Update is called once per frame
    public void Set(int terrorLevel)
    {
        for (int i = 0; i < icons.Length; ++i)
            icons[i].GetComponent<SpriteRenderer>().sprite = i >= terrorLevel ? goodSprite : badSprite;

    }
}
