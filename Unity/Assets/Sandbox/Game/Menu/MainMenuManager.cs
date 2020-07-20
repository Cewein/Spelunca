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


 
 public class MainMenuManager : MonoBehaviour
 {
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
     public GameObject optionLayout;
     
     public InputField usernameInput;
     public InputField passwordInput;
     public Animator[] loginInputFieldAnimators;

    

    public void Start()
    {
        string data = PlayerPrefs.GetString("player");
        PlayerData player = JsonUtility.FromJson<PlayerData>(data);
        if (player.id != "")
        {
            RestClient.Instance.isLoggedIn = true;
            refresh();
        }
    }

    public void quit()
    {
        Application.Quit();
    }
    public void openLeaderboard()
    {
        Application.OpenURL(RestClient.Instance.getServerUrl());
    }

    public void register()
    {
        Application.OpenURL(RestClient.Instance.getRegisterPageUrl());
    }
     
    private void refresh()
    {
        if (RestClient.Instance.isLoggedIn)
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

     public void showOptionView()
    {
        mainLayout.SetActive(false);
        optionLayout.SetActive(true);
    }

    public void hideOptionView()
    {
        mainLayout.SetActive(true);
        optionLayout.SetActive(false);
    }


    public void login()
    {
        LoginInfo loginInfo = new LoginInfo();
        loginInfo.username = usernameInput.text;
        loginInfo.password = passwordInput.text;
        StartCoroutine(RestClient.Instance.login(loginInfo, loginCallBack));
    }
    public void logout()
    {
        RestClient.Instance.logout();
        refresh();
    }

    private void loginCallBack(long responseCode, string jsonResult)
    {
        if (responseCode == 201)
        {
            PlayerPrefs.SetString("player",jsonResult);
            PlayerPrefs.Save();
            RestClient.Instance.isLoggedIn = true;
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
    }
 }
