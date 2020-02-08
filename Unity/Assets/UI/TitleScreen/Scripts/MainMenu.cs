using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string newGameScene;
    public int sceneIndex;
    public Text loadingField;
    
    public void newGame()
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
    public void quitGame()
    {
        Application.Quit();
    }
    
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f)*100f;
            loadingField.text = progress.ToString() + " %";
            yield return null;
        }
    }
}
