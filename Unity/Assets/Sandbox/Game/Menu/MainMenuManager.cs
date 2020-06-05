 using System;
 using System.Collections;
using System.Collections.Generic;
 using System.Text;
 using Newtonsoft.Json;
 using UnityEngine;
 using UnityEngine.Networking;
 using UnityEngine.SocialPlatforms;
 using UnityEngine.UI;
 using UnityEngine.UIElements;


 public struct LoginInfo
 {
     public string username;
     public string password;
 } 
 public struct PlayerData
 {
     public string id;
     public string username;
     public string email;
 } 
 
 public class MainMenuManager : MonoBehaviour
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

     [SerializeField] private EnvironmentMode environmentMode = EnvironmentMode.Local;
     private string localUrl = "http://localhost:8000/";
     private string preprodUrl = "http://13.80.137.233:8000/";
     private string prodUrl = "http://13.80.137.233/";

     private bool loggedIn = false;
     
     public void quit()
     {
         Application.Quit();
     }
     public void openLeaderboard()
     {
         Application.OpenURL(getServerUrl());
     }

     private void refresh()
     {
         if (loggedIn)
         {
             loginSubview.SetActive(false);
             loggedInSubview.SetActive(true);
             
             string jsonPlayer = PlayerPrefs.GetString("player");
             if (jsonPlayer != "")
             {
                 PlayerData player = JsonUtility.FromJson<PlayerData>(jsonPlayer);
                 usernameText.text = player.username;
             }else
             {
                 usernameText.text = "Error";
             }

         }else
         {
             loginSubview.SetActive(true);
             loggedInSubview.SetActive(false); 
         }
     }
     
     public void showLoginView()
     {
         mainLayout.SetActive(false);
         loginLayout.SetActive(true);
     }
     
     
     
     public void hideLoginView()
     {
         loginMessageText.gameObject.SetActive(false);
         mainLayout.SetActive(true);
         loginLayout.SetActive(false);
         refresh();
     }
     public void showCreditsView()
     {
         mainLayout.SetActive(false);
         creditsLayout.SetActive(true);
     }
     
     public void hideCreditsView()
     {
         mainLayout.SetActive(true);
         creditsLayout.SetActive(false);
     }

     public void login()
     {
         LoginInfo loginInfo = new LoginInfo();
         loginInfo.username = usernameInput.text;
         loginInfo.password = passwordInput.text;
         StartCoroutine(RestClient.Instance.login(getServerUrl() + KEY.URL_GET_USER, loginInfo, loginCallBack));
     }
     public void logout()
     {
         loggedIn = false;
         PlayerPrefs.DeleteKey("player");
         refresh();
     }

     private void loginCallBack(long responseCode, string jsonResult)
     {
         if (responseCode == 201)
         {
             PlayerPrefs.SetString("player",jsonResult);
             PlayerPrefs.Save();
             loggedIn = true;
             loginMessageText.gameObject.SetActive(true);
             loginMessageText.color = loginSuccessColor;
             loginMessageText.text = loginSuccessText;
             refresh();

         }else if (responseCode == 400)
         {
             loginMessageText.gameObject.SetActive(true);
             loginMessageText.color = loginErrorColor;
             loginMessageText.text = loginErrorText;
             foreach (Animator animator in loginInputFieldAnimators)
             {
                 animator.SetTrigger("error");
             }
         }
         
         //PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonResult);
         
     }
/*
     private IEnumerator login(LoginInfo loginInfo)
     {
         WWWForm form = new WWWForm();
         form.AddField("username", loginInfo.username);
         form.AddField("password", loginInfo.password);
         //var postData = JsonUtility.ToJson(loginInfo);
         //Debug.Log("data " + postData);
         UnityWebRequest www = UnityWebRequest.Post(getAPIUrl() + KEY.URL_GET_USER, form);
         www.uploadHandler.contentType = "application/json";
         
         //byte[] bytes = Encoding.Unicode.GetBytes(postData);
         //www.uploadHandler. = bytes;
         yield return www.SendWebRequest();

         if (www.isNetworkError || www.isHttpError)
         {
             Debug.Log(www.error);
         }
         else
         {
             Debug.Log("Form upload complete!");
             Debug.Log("Recieved : " + www.downloadHandler.text);
             
         }
     }*/

     /*var request = UnityWebRequest.Post(getAPIUrl() + KEY.URL_GET_USER, postData);
         var handler = request.SendWebRequest();
         while(!handler.isDone){
             yield return null;
         }

         switch (handler.webRequest.responseCode)
         {
             case 400:
                 errorLoginView();
                 break;
             case 201:
                 Debug.Log(request.downloadHandler.text);
                 break;
             case 500:
                 Debug.LogError("API call to '" + KEY.URL_GET_USER + "' : GET request prohibited.");
                 break;
         }*/
     
     private string getServerUrl()
     {
         switch (environmentMode)
         {
             case EnvironmentMode.Local:
                 return localUrl;
             case EnvironmentMode.Prod:
                 return prodUrl;
             case EnvironmentMode.PreProd:
                 return preprodUrl;
             default:
                 return "";
         }
     }
 }
