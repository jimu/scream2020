using UnityEngine;


public enum Music { None = -1, Title, Gameplay, Takedown };

/**
 * Usage:
 * 
 *   SoundManager.instance.PlayMusic(Music.None);
 *   SoundManager.instance.PlayMusic(Music.GamePlay);
 *   SoundManager.instance.PlayOneShot("event:/Placeholder SFX");
 * 
 */

public class SoundManager : MonoBehaviour
{
    public static SoundManager mainAudio;
    Music currentMusic = Music.None;
    bool isMusicSubdued = false;
    bool isMusicMuted = false;

    [SerializeField] float subduedVolume = 0.3f;

    public string music;




    void Awake()
    {
        if (mainAudio != null)
        {
            DestroyImmediate(this);
            return;
        }
        else
        {
            mainAudio = this;
            DontDestroyOnLoad(gameObject);
        }


    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ToggleMusic()
    {
        Debug.Log("toggle deez nuts");
    }
}