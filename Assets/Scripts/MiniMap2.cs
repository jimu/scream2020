using System.Collections;
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
        GameManager.instance.removeEnemyAction += RemoveEnemy;
        this.enemies = enemies;
        poolX = new RectTransform[enemies.Count];
        for(int i = 0; i < enemies.Count; ++i)
            poolX[i] = Instantiate(prefabX, transform).GetComponent<RectTransform>();

        playerMarker = Instantiate(prefabPlayerMarker, transform).GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(poolX[enemies.Count].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        const float scaleHackMult = 2.6f;  // scene3
        const float scaleHackX = 94f;
        const float scaleHackY = -95f;

        int i = 0;
        foreach(Enemy enemy in enemies)
        {
            //Vector3 pos = enemy.gameObject.transform.position * 1.05f;    // Map 1 1.05 scale
            Vector3 pos = enemy.gameObject.transform.position * scaleHackMult;
            //map xz from -90,90 to 90,-90 to 

            poolX[i].gameObject.SetActive(enemy.health > 0); // TODO: delete enemies and markers if dead
            poolX[i].anchoredPosition = new Vector2(pos.x + scaleHackX, pos.z + scaleHackY);
            i++;
        }
        playerMarker.anchoredPosition = new Vector2(player.position.x * scaleHackMult + scaleHackX, player.position.z * scaleHackMult + scaleHackY);
    }
}
