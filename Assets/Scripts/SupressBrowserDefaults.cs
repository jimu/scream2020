using UnityEngine;
using System.Runtime.InteropServices;


#pragma warning disable 0649
#pragma warning disable 0414

public class SupressBrowserDefaults : MonoBehaviour
{
    // Update is called once per frame
    private void Start()
    {
        InstallPreventDefaultF1();
    }

    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I: InstallPreventDefaultF1() -> PreventDefaultF1 in LibraryPreventDefaultF1 in PreventDefaultF1.jslib");
            InstallPreventDefaultF1();
        }
    }
    */

    public void InstallPreventDefaultF1()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            PreventDefaultF1();
#endif
    }

    [DllImport("__Internal")]
    private static extern void PreventDefaultF1();
}
