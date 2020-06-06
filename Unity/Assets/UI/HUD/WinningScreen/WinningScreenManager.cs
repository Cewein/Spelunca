 using System;
 using System.Collections;
using System.Collections.Generic;
 using System.Globalization;
 using System.Text;
 using Newtonsoft.Json;
 using UnityEngine;
 using UnityEngine.Networking;
 using UnityEngine.SceneManagement;
 using UnityEngine.SocialPlatforms;
 using UnityEngine.SocialPlatforms.Impl;
 using UnityEngine.UI;
 using UnityEngine.UIElements;


 
 public class WinningScreenManager : MonoBehaviour
 {
     

     public GameManager gameManager;
     public Text scoreText;
     public int mainMenuIndex = 0;
     public GameObject saveScoreButton;
     public GameObject registerButton;
     
     private bool loggedIn = false;
     private ScoreInfo score;
     
     public void loadMainMenu()
     {
         SceneManager.LoadScene(mainMenuIndex);
     }
     public void openLeaderboard()
     {
         Application.OpenURL(RestClient.Instance.getServerUrl());
     }

     public void OnEnable()
     {
         UnityEngine.Cursor.visible = true;
         UnityEngine.Cursor.lockState = CursorLockMode.None;
         score = gameManager.getFinalScore();
         if (RestClient.Instance.isLoggedIn)
         {
             saveScoreButton.SetActive(true);
             registerButton.SetActive(false);
         }
         else
         {
             registerButton.SetActive(true);
             saveScoreButton.SetActive(false);
         }
         scoreText.text = "Time : " + timeToString(score.time) + "\n\nEnemies killed : " + score.enemies + "\n\nHP lost : " + score.damage_taken;
     }

     private string timeToString(double time)
     {
         double s = time % 1;
         double ms = time - s;
         double m = s % 60;
         s = s - m * 60;
         double h = m % 60;
         m = m - h * 60;
         string score = "";
         score += (int)h > 0 ? (int)h + "h " : "";
         score += (int)m > 0 ? (int)m + "m " : "";
         score += (int)s > 0 ? (int)s + "s " : "";
         score += ms > 0 ? (int)(ms*1000) + "ms " : "";
         return score;
     }
     
     public void register()
     {
         Application.OpenURL(RestClient.Instance.getRegisterPageUrl());
     }
     public void saveScore()
     {
         string playerData = PlayerPrefs.GetString("player");
         PlayerData player = JsonUtility.FromJson<PlayerData>(playerData);
         if (player.id != "")
         {
             score.player = player.id;
             StartCoroutine(RestClient.Instance.saveScore(score, scoreCallBack));  
         }
         
     }

     private void scoreCallBack(long responseCode, string jsonResult)
     {
         if (responseCode == 201)
         {
             Debug.Log("Score saved in database !");
         }else if (responseCode == 400)
         {
             Debug.Log("Oops... An error occured...");
         }
     }
 }
