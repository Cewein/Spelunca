 using System;
 using System.Collections;
using System.Collections.Generic;
 using System.Text;
 using Newtonsoft.Json;
 using UnityEngine;
 using UnityEngine.Networking;
 using UnityEngine.SocialPlatforms;
 using UnityEngine.UI;


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
         internal static readonly string URL_GET_USER = "get_user/";
     }
     
     public GameObject mainLayout;
     public GameObject loginLayout;
     public GameObject creditsLayout;
     
     public InputField usernameInput;
     public InputField passwordInput;
     public Animator[] loginInputFieldAnimators;

     [SerializeField] private EnvironmentMode environmentMode = EnvironmentMode.Local;
     private string localUrl = "localhost:8000/api/";
     private string preprodUrl = "13.80.137.233:8000/api/";
     private string prodUrl = "13.80.137.233/api/";

     public void quit()
     {
         Application.Quit();
     }
     public void showLoginView()
     {
         mainLayout.SetActive(false);
         loginLayout.SetActive(true);
     }
     
     public void hideLoginView()
     {
         mainLayout.SetActive(true);
         loginLayout.SetActive(false);
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
         StartCoroutine(RestClient.Instance.login(getAPIUrl() + KEY.URL_GET_USER, loginInfo, loginCallBack));
     }

     private void loginCallBack(PlayerData player)
     {
         Debug.Log("Callback ! Id : " + player.id);
     
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
     
     private void errorLoginView()
     {
         foreach (Animator animator in loginInputFieldAnimators)
         {
             animator.SetTrigger("error");
         }
     }

     private string getAPIUrl()
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
