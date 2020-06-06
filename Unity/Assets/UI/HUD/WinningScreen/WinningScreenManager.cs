 using System;
 using System.Collections;
using System.Collections.Generic;
 using System.Text;
 using Newtonsoft.Json;
 using UnityEngine;
 using UnityEngine.Networking;
 using UnityEngine.SceneManagement;
 using UnityEngine.SocialPlatforms;
 using UnityEngine.UI;
 using UnityEngine.UIElements;


 
 public class WinningScreenManager : MonoBehaviour
 {
     
     private enum EnvironmentMode {Local,PreProd,Prod}
     private struct KEY
     {
         internal static readonly string URL_GET_USER = "api/get_user/";
     }
     
     public GameObject mainLayout;
     
     public GameObject loginLayout;
     public GameObject loginSubview;
     public GameObject loggedInSubview;
     public Text usernameText;
     public Text loginMessageText;
     public string loginSuccessText = "Successfully logged in !";
     public Color loginSuccessColor = Color.green;
     public string loginErrorText = "Please enter a correct username and password.";
     public Color loginErrorColor = Color.red;
     
     public GameObject creditsLayout;
     
     public InputField usernameInput;
     public InputField passwordInput;
     public Animator[] loginInputFieldAnimators;
     public int mainMenuIndex = 0;

     private bool loggedIn = false;
     
     public void loadMainMenu()
     {
         SceneManager.LoadScene(mainMenuIndex);
     }
     public void openLeaderboard()
     {
         Application.OpenURL(RestClient.Instance.getServerUrl());
     }

     public void saveScore()
     {
         ScoreInfo scoreInfo = new ScoreInfo();
         string playerData = PlayerPrefs.GetString("player");
         PlayerData player = JsonUtility.FromJson<PlayerData>(playerData);
         scoreInfo.player = player.id;
         scoreInfo.time = "154";
         scoreInfo.enemies = "487";
         scoreInfo.damage_taken = "487";
         StartCoroutine(RestClient.Instance.saveScore(scoreInfo, scoreCallBack));
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
