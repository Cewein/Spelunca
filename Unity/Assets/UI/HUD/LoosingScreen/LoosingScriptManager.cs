using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoosingScriptManager : MonoBehaviour
{
    public int mainMenuIndex = 0;
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuIndex);
    }
}
