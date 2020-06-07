using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{



    [SerializeField] private PlayerStats player = null;
    [SerializeField] private GameObject[] mainUI = null;
    [SerializeField] private GameObject gameOverScreen = null;
    
    [SerializeField] private GameObject winScreen = null;
    public string mainMenuPath;
    public string gameScenePath;
    private bool playerIsDead = false;
    private int count = 300;
    public bossScript boss;
    
    //Score

    private ScoreInfo score;
    private float startTime;
    private bool scoreHasStarted = false;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go =  new GameObject();
                    go.name = typeof(RestClient).Name;
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        score = new ScoreInfo();
        DebugResourcesStockNotLoading();

        if (player != null) player.die += PlayerDie;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

    }
    private IEnumerator DebugResourcesStockNotLoading()
    {
        yield return new WaitForEndOfFrame();
        if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void PlayerDie(bool isDie)
    {
        gameOverScreen.SetActive(true);
        foreach(GameObject ui in mainUI)
        {
            ui.SetActive(false);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerIsDead = true;
    }

    private void Update()
    {
        if (boss == null && winScreen !=null)
        {
            winScreen.SetActive(true);
            foreach(GameObject ui in mainUI)
            {
                ui.SetActive(false);
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //count--;   
            //if (count<1)LoadLevel(mainMenuPath);
        }
    }

    
    void LoadLevel(string path)
    {          
        SceneManager.LoadScene(path);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartNewGame()
    {
        LoadLevel(gameScenePath);
    }

    public void StartNewScore()
    {
        scoreHasStarted = true;
        startTime = Time.time;
        score = new ScoreInfo();
    }

    public void EnemyKilled()
    {
        if(scoreHasStarted)score.enemies += 1;
    }
    public void HPLost(float hpLost)
    {
        if(scoreHasStarted)score.damage_taken += hpLost;
    }
    public ScoreInfo getFinalScore()
    {
        scoreHasStarted = false;
        float now = Time.time;
        score.time = now - startTime;
        return score;
    }
}