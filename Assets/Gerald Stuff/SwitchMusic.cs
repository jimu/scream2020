using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusic : MonoBehaviour
{
    public AudioClip newTrack;

    private SoundManager theSM;

    // Start is called before the first frame update
    void Start()
    {
        theSM = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MusicChange()
    {
         if (newTrack != null)
        {
            theSM.ChangeBGM(newTrack);
        }
    }
}
