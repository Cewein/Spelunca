using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Security.Cryptography;
using System.Text;

public enum EnvironmentMode {Local,PreProd,Prod}

public struct LoginInfo
{
    public string username;
    public string password;
} 
 
public struct ScoreInfo
{
    public int player;
    public double time;
    public int enemies;
    public float damage_taken;
} 
public struct PlayerData
{
    public string id;
    public string username;
    public string email;
} 
public class RestClient : MonoBehaviour
{
    private static RestClient _instance;

    public EnvironmentMode environmentMode = EnvironmentMode.Prod;
    private string localUrl = "http://localhost:8000/";
    private string preprodUrl = "http://13.80.137.233:8000/";
    private string prodUrl = "http://13.80.137.233/";
    public bool isLoggedIn = false;
    private struct KEY
    {
        internal static readonly string URL_GET_USER = "api/get_user/";
        internal static readonly string URL_SAVE_SCORE = "api/save_score/";
        internal static readonly string URL_REGISTER = "register/";
    }

    public static RestClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RestClient>();
                if (_instance == null)
                {
                    GameObject go =  new GameObject();
                    go.name = typeof(RestClient).Name;
                    _instance = go.AddComponent<RestClient>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public IEnumerator login(LoginInfo loginInfo, System.Action<long,string> callBack)
    {
        SHA256 sha256Hash = SHA256.Create();
        loginInfo.password = GetHash(sha256Hash, loginInfo.password);

        string url = getServerUrl() + KEY.URL_GET_USER;
        var jsonData = JsonUtility.ToJson(loginInfo);
        yield return Post(url, jsonData, callBack);
    }

    public void logout()
    {
        isLoggedIn = false;
        PlayerPrefs.DeleteKey("player");
    }
    
    public IEnumerator saveScore(ScoreInfo score, System.Action<long,string> callBack)
    {
        string url = getServerUrl() + KEY.URL_SAVE_SCORE;
        var jsonData = JsonUtility.ToJson(score);
        Debug.Log("Saving score to database as JSON : " + jsonData);
        yield return Post(url, jsonData, callBack);
    }

    private IEnumerator Post(string url,string jsonData, System.Action<long,string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url,jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.uploadHandler.contentType = "application/json";
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                //Debug.Log(www.error);
                callBack(www.responseCode, null);
            }
            else if (www.isHttpError)
            {
                string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                //Debug.Log(jsonResult);
                callBack(www.responseCode,jsonResult);
            }else if (www.isDone)
            {
                string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                //Debug.Log(jsonResult);
                callBack(www.responseCode,jsonResult);
            }
        }
    }

    public string getRegisterPageUrl()
    {
        return getServerUrl() + KEY.URL_REGISTER;
    }
    public string getServerUrl()
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