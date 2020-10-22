using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text scoreText;
    [SerializeField] UnityEngine.UI.Text endReason;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("EndGame" + scoreText == null ? "null" : "notnull" + " score=" + GameManager.instance.GetScore().ToString());
        scoreText.text = GameManager.instance.GetScore().ToString();
        endReason.text = GameManager.instance.GetEndReason();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
