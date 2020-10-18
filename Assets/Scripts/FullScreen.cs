using UnityEngine;
using System.Runtime.InteropServices;

#pragma warning disable 0649
#pragma warning disable 0414

public class FullScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H");
            ActivateFullscreen();
        }
    }

    public void ToggleFullscreen()
    {
        ActivateFullscreen(!Screen.fullScreen);
    }


    public void ActivateFullscreen(bool isFullScreen = true)
    {
        if (!isFullScreen)
            Screen.fullScreen = false;
        else
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GoFullscreen(isFullScreen);
#else
            Screen.fullScreen = true;
#endif
        }

    }

    [DllImport("__Internal")]
    private static extern void GoFullscreen(bool isFullScreen);
}
