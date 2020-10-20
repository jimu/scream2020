﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 0649

enum GameState { Invalid, Start, Playing, Help, GameOver, HighScores }

public class GameManager : MonoBehaviour
{
    enum CameraTarget { Player, Green };

    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject miniMap;
    [SerializeField] GameObject hud;
    [SerializeField] MiniMap2 miniMap2;
    [SerializeField] Text scoreText;
    [SerializeField] Sprite[] borderGraphics;
    int currentBorderGraphic = 0;
    [SerializeField] Image borderGraphic;
    bool miniMapVisible = true;

    List<GameObject> exits;
    
    public List<Enemy> enemies;
    public Action<Enemy> removeEnemyAction = null;

    [SerializeField] GameObject debugPanel;
    [SerializeField] float fovZoomIn  = 60;
    [SerializeField] float fovZoomOut = 90;
    [SerializeField] GameObject progressBar;
    public PredatorVision predatorVision;

    Camera mainCamera;

    AudioSource audioSource;

    // Playtesting
    bool useCylinder = false;

    private GameState state = GameState.Playing;
    private int score = 0;

    static public GameManager instance;


    CameraTarget cameraTarget = CameraTarget.Player;
    int terrorLevel = 0;
    static readonly int MaxTerrorLevel = 5;


    private void Awake()
    {
        GameManager.instance = this;
        predatorVision = GetComponent<PredatorVision>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Init();
    }

    private void Init()
    {
        SetState(GameState.Start);
        SetScore(0);

        exits = new List<GameObject>();
        foreach (GameObject exit in GameObject.FindGameObjectsWithTag("Exit"))
            exits.Add(exit);

        enemies = new List<Enemy>();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemy = o.GetComponent<Enemy>();
            enemies.Add(enemy);

            MoveTo moveTo = o.GetComponent<MoveTo>();
            moveTo.goal = exits[UnityEngine.Random.Range(0, exits.Count)].transform;
        }
        miniMap2.Init(enemies); // initialize minimap with our new enemy listss
    }
    
    public void removeEnemy(Enemy enemy)
    {
        removeEnemyAction(enemy);
        enemies.Remove(enemy);
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
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

    public int GetTerrorLevel()
    {
        return terrorLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Escape))
            SetState(state == GameState.Help ? GameState.Playing : GameState.Help);
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.F2))
            SetCameraTarget((CameraTarget)(((int)cameraTarget + 1) % System.Enum.GetNames(typeof(CameraTarget)).Length));
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.F3))
            SetTerrorLevel((terrorLevel + 1) % MaxTerrorLevel);
        if (Input.GetKeyDown(KeyCode.F1))
            debugPanel.SetActive(!debugPanel.activeSelf);
        if (Input.GetKeyDown(KeyCode.F4))
            CycleBorderGraphic();
        if (Input.GetKeyDown(KeyCode.F5))
            CycleCamperModel();
        if (Input.GetKeyDown(KeyCode.F6))
            ToggleMiniMap();
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            IncrementFOV(-5);
        if (Input.GetKeyDown(KeyCode.Minus))
            IncrementFOV(5);
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetButtonDown("Button5")) // right bumper
            ToggleFOV();

        float analogZoom = Input.GetAxis("Left Trigger") - Input.GetAxis("Right Trigger") + Input.GetAxis("Right Joystick Vertical");
        if (analogZoom > 0.01 || analogZoom < -0.01)
            IncrementFOV(analogZoom);
    }
    void CycleCamperModel()
    {
        useCylinder = !useCylinder;
        foreach (Enemy e in enemies)
        {
            if (e.gameObject.transform.childCount > 0)
            {

                GameObject child = e.gameObject.transform.GetChild(0).gameObject;
                child.SetActive(!useCylinder);
                child.GetComponent<Animator>().SetBool("Death", e.health < 1);
            }
            e.gameObject.GetComponent<MeshRenderer>().enabled = useCylinder;
        }
    }

    void CycleBorderGraphic()
    {
        currentBorderGraphic = (currentBorderGraphic + 1) % (borderGraphics.Length + 1);
        borderGraphic.enabled = currentBorderGraphic < borderGraphics.Length;
        if (currentBorderGraphic < borderGraphics.Length)
            borderGraphic.sprite = borderGraphics[currentBorderGraphic];
    }


    void ToggleMiniMap()
    {
        miniMapVisible = !miniMapVisible;
        miniMap2.gameObject.SetActive(miniMapVisible);
    }


    void SetState(GameState state)
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
        SetState(GameState.Playing);
    }


    public void OnFullscreenPressed()
    {
        // This should come first?
        GameObject.Find("FullScreenButtonText").GetComponent<Text>().text = Screen.fullScreen ?  "Fullscreen" : "Windowed";
        GetComponent<FullScreen>().ActivateFullscreen(!Screen.fullScreen);
        //GetComponent<FullScreen>().ActivateFullscreen();
        //setState(GameState.Playing);
    }

    public void AddScore(int value)
    {
        SetScore(score + value);
    }

    private void SetScore(int value)
    {
        score = value;
        scoreText.text = value.ToString();
    }

    void IncrementFOV(float value)
    {
        mainCamera.fieldOfView += value;
    }

    void ToggleFOV()
    {
        mainCamera.fieldOfView = mainCamera.fieldOfView == fovZoomIn ? fovZoomOut : fovZoomIn;
    }


    public void RecalculateNavigation()
    {
        foreach(Enemy enemy in enemies)
            enemy.RecalculateNavigation();
    }

    public void StartProgressBar(float duration)
    {
        Debug.Log("StartProgressBar(" + duration + ")");
        progressBar.SetActive(true);
        progressBar.GetComponent<ProgressBar>().StartProgress(duration);
    }

    public void StopProgressBar()
    {
        progressBar.SetActive(false);
    }
}
