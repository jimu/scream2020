using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 0649

public enum GameState { Invalid, Start, Playing, Paused, Help, GameOver, HighScores }

public class GameManager : MonoBehaviour
{
    enum CameraTarget { Player, Green };

    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject miniMap;
    [SerializeField] GameObject hud;
    [SerializeField] MiniMap2 miniMap2;
    [SerializeField] Text scoreText;
    [SerializeField] Sprite[] borderGraphics;
    int currentBorderGraphic = 0;
    [SerializeField] Image borderGraphic;
    [SerializeField] float wanderPhaseDuration = 60.0f;


    bool miniMapVisible = true;

    List<GameObject> exits;
    
    public List<Enemy> enemies;
    public Action<Enemy> removeEnemyAction = null;
    string endReason = "Error";


    [SerializeField] GameObject debugPanel;
    [SerializeField] GameObject progressBar;

    [Header("Game Designer Stettings")]
    [Tooltip("How Long the game round lasts in seconds")]
    [SerializeField] float sceneDuration;
    [SerializeField] float fovZoomIn = 60;
    [SerializeField] float fovZoomOut = 90;

    Vector3[] wanderPoints;

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
        audioSource = GetComponent<AudioSource>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("WanderPoint");
        wanderPoints = new Vector3[objects.Length];
        for (int i = 0; i < objects.Length; ++i)
            wanderPoints[i] = objects[i].transform.position;
    }


    public Vector3 getRandomWanderPoint()
    {
        return wanderPoints[UnityEngine.Random.Range(0, wanderPoints.Length)];
    }

    // wait as long as posible so enemies can awake
    private void Start()
    {
        Init();
        SetState(GameState.Start);
    }


    private void Init()
    {
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
            moveTo.SetExitPosition(exits[UnityEngine.Random.Range(0, exits.Count)].transform.position);
        }
        miniMap2.Init(enemies); // initialize minimap with our new enemy listss
    }

    public GameState GetGameState()
    {
        return state;
    }

    public void RemoveEnemy(Enemy enemy)
    {
       // Debug.Log("GameManager.RemoveEnemy: " + enemy.gameObject.name);

        removeEnemyAction(enemy);
        enemies.Remove(enemy);
        //Debug.Log("GameManager.RemoveEnemy: " + enemy.gameObject.name + " DONE");

        if (enemies.Count < 1)
            EndRound("All campers killed or escaped");

    }

    public void PlayOneShot(AudioClip audioClip)
    {
        //Debug.Log("PlayOneShot:" + audioClip.name);
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayOneShotIfGamePlay(AudioClip audioClip)
    {
        if (state == GameState.Playing)
            PlayOneShot(audioClip);
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
        if (state == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Escape))
                SetState(
                    state == GameState.Start ? GameState.Help :
                    state == GameState.Help ? GameState.Start :
                    state == GameState.Paused ? GameState.Playing :
                    GameState.Paused);
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
            if (Input.GetKeyDown(KeyCode.Home))
                //  SoundManager.mainAudio.PlayOneShot("event:/Placeholder SFX");
                if (Input.GetKeyDown(KeyCode.M))
                    SoundManager.mainAudio.ToggleMusic();

            float analogZoom = Input.GetAxis("Left Trigger") - Input.GetAxis("Right Trigger") + Input.GetAxis("Right Joystick Vertical");
            if (analogZoom > 0.01 || analogZoom < -0.01)
                IncrementFOV(analogZoom);

            if (Time.timeSinceLevelLoad > sceneDuration)
                EndRound("Out of time");

            if (Time.timeSinceLevelLoad > wanderPhaseDuration)
                Enemy.CancelAllWandering();
        }
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
        Debug.Log("SetState: " + state);
        // Hack so "help" on Title will have title music while pausing ingame will have GamePlay music
        if (this.state == GameState.Start && state == GameState.Paused)
            state = GameState.Help;

        //Debug.Log("setState(" + state.ToString() + ")");
        this.state = state;

        Time.timeScale = state == GameState.Playing ? 1f : 0f;
        Debug.Log("  Time.timeScale=" + Time.timeScale);

        startPanel.SetActive(state == GameState.Start);
        helpPanel.SetActive(state == GameState.Help || state == GameState.Paused);
        gameOverPanel.SetActive(state == GameState.GameOver);
        miniMap.SetActive(state == GameState.Playing);
        hud.SetActive(state == GameState.Playing);

        Music music =
            state == GameState.Start ? Music.Title :
            state == GameState.Help ? Music.Title :
            state == GameState.Playing ? Music.Gameplay :
            state == GameState.Paused ? Music.Gameplay :
            Music.None;


      //  SoundManager.instance?.SetSubduedMusic(state == GameState.Paused);
      //  SoundManager.instance?.PlayMusic(music);

    }

    public void OnRestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //SetState(state == GameState.Help ? GameState.Start : GameState.Start);
    }


    public void OnClosePressed()
    {
        SetState(state == GameState.Help ? GameState.Start : GameState.Playing);
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

    public int GetScore()
    {
        return score;
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

    public float GetSceneDuration()
    {
        return sceneDuration;
    }

    public string GetEndReason()
    {
        return endReason;
    }

    void EndRound(string reason)
    {
        Debug.Log("EndRound(" + reason + ")");
        endReason = reason;
        SetState(GameState.GameOver);
    }


}
