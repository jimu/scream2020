using UnityEngine;

public class StartMenu : MonoBehaviour
{
    /*
    [EventRef] public string musicEventRef;
    public FMOD.Studio.EventInstance eventInstance;

    void Awake()
    {
        RuntimeManager.CreateInstance(musicEventRef);
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(musicEventRef);
        eventInstance.setParameterByName("Music_State", 0); // 0 = title music
        eventInstance.start();
        Debug.Log("Title music should be playing");
    }
    */

    private void Start()
    {
        Awake();
    }

    private void Awake()
    {

    }
}
