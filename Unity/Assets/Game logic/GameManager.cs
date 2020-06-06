﻿using System;
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
    [SerializeField] private GameObject[] mainUI = null;
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
            //count--;   
            //if (count<1)LoadLevel(mainMenuPath);
        }
    }

    public ScoreInfo getFinalScore()
    {
        //todo : un timer, un compteur à araignées et un compteur à pv perdu
        ScoreInfo score = new ScoreInfo();
        score.time = 1758.15473;
        score.enemies = 42;
        score.damage_taken = 145.2f;
        return score;
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
}