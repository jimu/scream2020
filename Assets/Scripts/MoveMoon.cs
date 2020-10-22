using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMoon : MonoBehaviour
{
    [Tooltip("Starting position of moon")]
    [SerializeField] Transform startPosition;

    [Tooltip("Ending position of moon")]
    [SerializeField] Transform endPosition;
    
    [Header("Duration of scene in seconds")]
    [SerializeField] float duration;

    Vector3 startPos;
    Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = startPosition.position;
        endPos = endPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(startPos, endPos, Time.time / duration);
    }
}
