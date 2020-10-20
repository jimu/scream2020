using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager0 : MonoBehaviour
{
    public static SoundManager0 mainAudio;

    [SerializeField]
    private string bulletFire;

    [FMODUnity.EventRef] public string music;

    public FMOD.Studio.EventInstance bulletFireEvent;   // not used
    public FMOD.Studio.EventInstance musicEvent;

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

    void Start()
    {
        musicEvent = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEvent.start();

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
