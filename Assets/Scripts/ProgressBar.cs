using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    float startTime;
    Image loadingBar;
    float duration = 1f;

    private void Awake()
    {
        loadingBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void StartProgress(float duration)
    {
        loadingBar.fillAmount = 0f;
        this.duration = duration;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        loadingBar.fillAmount = (Time.time - startTime) / duration;
    }
}
