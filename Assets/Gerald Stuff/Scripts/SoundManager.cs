using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager mainAudio;

    [SerializeField]
    private string bulletFire;

    [SerializeField]
    private string heartBeatSound;

    public FMOD.Studio.EventInstance bulletFireEvent;

    // Start is called before the first frame update
    void Awake()
    {
        if (mainAudio != null)
        {
            DestroyImmediate(this);
        }
        else
            mainAudio = this;
        DontDestroyOnLoad(gameObject);


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Key Pressed");
            FMODUnity.RuntimeManager.PlayOneShot(bulletFire);
        }
    }
}
