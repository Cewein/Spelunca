using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ResourcesStock resourcesStock;
    [SerializeField]
    private ConsumableStock consumableStock;

    public static GameManager instance = null;
    [SerializeField] private PlayerStats player = null;
    [SerializeField] private GameObject gameOverScreen = null;
    [SerializeField] private GameObject winScreen = null;
    public string mainMenuPath;
    public string gameScenePath;
    private bool playerIsDead = false;
    private int count = 300;
    public bossScript boss;
    void Awake()
    {
        DebugResourcesStockNotLoading();

        if (player != null) player.die += PlayerDie;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

    }
    private IEnumerator DebugResourcesStockNotLoading()
    {
        yield return new WaitForEndOfFrame();
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        //  initRun();
    }

    void PlayerDie(bool isDie)
    {
        gameOverScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerIsDead = true;
    }

    private void Update()
    {
        if (boss == null && winScreen !=null)
        {
            winScreen.SetActive(true);
            count--;   
            if (count<1)LoadLevel(mainMenuPath);
        }

        if (playerIsDead)
        {
            count--;   
            if (count<1)LoadLevel(mainMenuPath);
        }
    }

    void LoadLevel(string path)
    {          
        SceneManager.LoadScene(path);
    }

    public void StartNewGame()
    {
        LoadLevel(gameScenePath);
    }
}