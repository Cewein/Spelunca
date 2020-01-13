using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreloadManager : MonoBehaviour
{
    public String speech;
    public Text text;
    public int sceneIndex;
    void Start()
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);
            yield return null;
        }
    }
    
    IEnumerator loading()
    {
        while (true)
        {
            text.text = speech;
            yield return new WaitForSeconds(0.33f);
            text.text = speech + " .";
            yield return new WaitForSeconds(0.33f);
            text.text = speech + " ..";
            yield return new WaitForSeconds(0.33f);
            text.text = speech + " ...";
            yield return new WaitForSeconds(0.33f);
        }
    }
}
