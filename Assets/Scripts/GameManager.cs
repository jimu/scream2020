using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GameState { Invalid, Start, Playing, Help, GameOver, HighScores }

public class GameManager : MonoBehaviour
{
    enum CameraTarget { Player, Green };

    public GameObject helpPanel;
    public GameObject startPanel;
    public GameObject miniMap;
    public GameObject hud;

    private GameState state = GameState.Playing;



    CameraTarget cameraTarget = CameraTarget.Player;
    int terrorLevel = 0;
    static readonly int MaxTerrorLevel = 5;


    private void Start()
    {
        setState(GameState.Start);
    }

    void SetCameraTarget(CameraTarget newCameraTarget)
    {
        cameraTarget = newCameraTarget;

        Debug.Log("SetCameraTarget: " + cameraTarget.ToString());
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().FollowTarget(
            GameObject.FindGameObjectWithTag(cameraTarget.ToString()).transform,
            cameraTarget == CameraTarget.Green ? Follow.Mode.Spy : Follow.Mode.Track);
    }

    public void SetTerrorLevel(int n)
    {
        terrorLevel = n;
        GameObject.Find("TerrorMeter").GetComponent<TerrorMeter>().Set(terrorLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F1))
            setState(state == GameState.Help ? GameState.Playing : GameState.Help);
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.F2))
            SetCameraTarget((CameraTarget)(((int)cameraTarget + 1) % System.Enum.GetNames(typeof(CameraTarget)).Length));
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.F3))
            SetTerrorLevel((terrorLevel + 1) % MaxTerrorLevel);
    }

    void setState(GameState state)
    {
        //Debug.Log("setState(" + state.ToString() + ")");
        this.state = state;

        Time.timeScale = state == GameState.Playing ? 1f : 0f;

        startPanel.SetActive(state == GameState.Start);
        helpPanel.SetActive(state == GameState.Help);
        miniMap.SetActive(state == GameState.Playing);
        hud.SetActive(state == GameState.Playing);
    }

    public void OnClosePressed()
    {
        setState(GameState.Playing);
    }
    public void OnFullscreenPressed()
    {
        // This should come first?
        GameObject.Find("FullScreenButtonText").GetComponent<Text>().text = Screen.fullScreen ? "Windowed" : "Fullscreen";
        GetComponent<FullScreen>().ActivateFullscreen(!Screen.fullScreen);
        //GetComponent<FullScreen>().ActivateFullscreen();
        //setState(GameState.Playing);
    }
}
