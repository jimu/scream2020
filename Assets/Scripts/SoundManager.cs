using UnityEngine;


//public enum Music { None = -1, Title, Gameplay, Takedown };

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
    //Music currentMusic = Music.None;
    // bool isMusicSubdued = false;
    // bool isMusicMuted = false;

    //   [SerializeField] float subduedVolume = 0.3f;

    public AudioSource BGM;


    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    void Update()
    {

    }

    public void ChangeBGM(AudioClip music)
    {
       // if(BGM.clip.name == music.name)
        ///{
          //  return;
        //}

        BGM.Stop();
        BGM.clip = music;
        BGM.Play();
    }
}