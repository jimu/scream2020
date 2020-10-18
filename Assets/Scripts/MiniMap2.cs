﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class MiniMap2 : MonoBehaviour
{
    [SerializeField] GameObject prefabX;
    [SerializeField] GameObject prefabPlayerMarker;
    List<Enemy> enemies;
    RectTransform[] poolX;
    RectTransform playerMarker;
    Transform player;

    public void Init(List<Enemy> enemies)
    {
        this.enemies = enemies;
        poolX = new RectTransform[enemies.Count];
        for(int i = 0; i< enemies.Count; ++i)
            poolX[i] = Instantiate(prefabX, transform).GetComponent<RectTransform>();

        playerMarker = Instantiate(prefabPlayerMarker, transform).GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach(Enemy enemy in enemies)
        {
            Vector3 pos = enemy.gameObject.transform.position * 1.05f;
            //map xz from -90,90 to 90,-90 to 

            poolX[i].anchoredPosition = new Vector2(pos.x + 5 , pos.z);
            i++;
        }
        playerMarker.anchoredPosition = new Vector2(player.position.x + 5, player.position.z);
    }
}
