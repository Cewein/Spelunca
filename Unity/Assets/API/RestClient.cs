using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;

public class RestClient : MonoBehaviour
{
    private static RestClient _instance;

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
/*
    public IEnumerator Get(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log(jsonResult);
                }
            }
        }
    }*/
    
    public IEnumerator login(string url,LoginInfo loginInfo, System.Action<PlayerData> callBack)
    {
        var jsonData = JsonUtility.ToJson(loginInfo);
        using (UnityWebRequest www = UnityWebRequest.Post(url,jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.uploadHandler.contentType = "application/json";
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log(jsonResult);
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonResult);
                    callBack(playerData);
                }
            }
        }
    }
}