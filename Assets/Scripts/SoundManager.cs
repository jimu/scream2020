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
    public static SoundManager instance;
    Music currentMusic = Music.None;
    bool isMusicSubdued = false;
    bool isMusicMuted = false;

    [SerializeField] float subduedVolume = 0.3f;

    [FMODUnity.EventRef] public string music = "event:/Music 2";

    FMOD.Studio.EventInstance musicEventInstance;
    

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicEventInstance = FMODUnity.RuntimeManager.CreateInstance(music);
    }




    public void PlayMusic(Music music)
    {
        if (currentMusic != music)
        {
            if (music == Music.None)
                musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            else
            {
                musicEventInstance.setParameterByName("Music_State", (int)music); // 0 = title music
                musicEventInstance.start();
            }
            Debug.Log(string.Format("Music ({0}) playing", music));
            currentMusic = music;
        }
    }

    public void PlayOneShot(string sfxEventRef)
    {
        FMODUnity.RuntimeManager.PlayOneShot(sfxEventRef);
    }
    public void SetMusicVolume(float volume)
    {
        musicEventInstance.setVolume(volume);
    }

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        musicEventInstance.setVolume(isMusicMuted ? 0f : isMusicSubdued ? subduedVolume : 1f);
    }

    public void SetSubduedMusic(bool isSubdued = true)
    {
        if (isSubdued != isMusicSubdued)
        {
            isMusicSubdued = isSubdued;
            if (!isMusicMuted)
                musicEventInstance.setVolume(isMusicSubdued ? subduedVolume : 1f);
        }
    }
}