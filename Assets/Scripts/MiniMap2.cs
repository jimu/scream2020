using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class MiniMap2 : MonoBehaviour
{
    [SerializeField] GameObject prefabX;
    [SerializeField] GameObject prefabPlayerMarker;
    [SerializeField] GameObject prefabLogIcon;
    List<Enemy> enemies;
    RectTransform[] poolX; // TODO: should be list or not exist at all
    int poolXcount;

    // Logs
    RectTransform[] logIcons;
    [SerializeField] Sprite uprightLogSprite;
    [SerializeField] Sprite blockingLogSprite;

    RectTransform playerMarker;
    Transform player;

    public void Init(List<Enemy> enemies)
    {
        GameManager.instance.removeEnemyAction += RemoveEnemy;
        this.enemies = enemies;
        poolX = new RectTransform[enemies.Count];
        for(int i = 0; i < enemies.Count; ++i)
            poolX[i] = Instantiate(prefabX, transform).GetComponent<RectTransform>();

        poolXcount = enemies.Count;
        playerMarker = Instantiate(prefabPlayerMarker, transform).GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;


        // log-icons (trees)
        int numLogs = GameObject.FindGameObjectsWithTag("Log").Length;
        logIcons = new RectTransform[numLogs];
        for (int i = 0; i < numLogs; ++i)
            logIcons[i] = Instantiate(prefabLogIcon, transform).GetComponent<RectTransform>();
        UpdateLogs();

    }

    public void RemoveEnemy(Enemy enemy)
    {
        //Debug.Log("MiniMap2.RemoveEnemy:" + enemy.gameObject.name + " b4:"+ enemies.Count + "/" + poolXcount);
        //foreach (Enemy e in enemies)
        //    Debug.Log(e.gameObject.name); enemies.Remove(enemy);
        poolXcount--;
        Destroy(poolX[poolXcount].gameObject);
        poolX[poolXcount] = null;
        //Debug.Log("MiniMap2.RemoveEnemy:" + enemy.gameObject.name + " After:" + enemies.Count + "/" + poolXcount);
        //foreach (Enemy e in enemies)
        //    Debug.Log(e.gameObject.name);
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
            if (poolX[i] == null)
            {
                Debug.Log("Fail:" + enemy.gameObject.name + " After:" + enemies.Count + "/" + poolXcount);
                Debug.Break();
            }

            poolX[i].gameObject.SetActive(enemy.health > 0); // TODO: delete enemies and markers if dead
            poolX[i].anchoredPosition = new Vector2(pos.x + scaleHackX, pos.z + scaleHackY);
            i++;
        }
        playerMarker.anchoredPosition = new Vector2(player.position.x * scaleHackMult + scaleHackX, player.position.z * scaleHackMult + scaleHackY);
    }

    const float scaleHackMult = 2.6f;  // scene3
    const float scaleHackX = 94f;
    const float scaleHackY = -95f;

    public void UpdateLogs()
    {
//        Debug.Log("Logs.Update() " + GameObject.FindGameObjectsWithTag("Log").Length + " logs");
        int i = 0;
        foreach (GameObject log in GameObject.FindGameObjectsWithTag("Log"))
        {
//            Debug.Log("   " + log.name);

            bool isBlocking = log.GetComponent<Log>().IsBlocking();
            Vector3 pos = log.gameObject.transform.position * scaleHackMult;
            logIcons[i].anchoredPosition = new Vector2(pos.x + scaleHackX, pos.z + scaleHackY);
            logIcons[i].GetComponent<UnityEngine.UI.Image>().sprite = isBlocking ? blockingLogSprite : uprightLogSprite;
            i++;
//            Debug.Log("updated log " + log.name);
        }

    }
}
