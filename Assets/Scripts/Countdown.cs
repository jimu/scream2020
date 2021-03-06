﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    Text timeText;
    [SerializeField] int startTime = 2 * 60 + 30;  // 2:30

    // Start is called before the first frame update
    void Awake()
    {
        timeText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int seconds = startTime - Mathf.FloorToInt(Time.timeSinceLevelLoad);
        timeText.text = seconds / 60 + ":" + (seconds % 60).ToString().PadLeft(2, '0');
    }
}
