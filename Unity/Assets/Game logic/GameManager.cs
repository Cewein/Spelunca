using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    [SerializeField] private PlayerStats player = null;
    [SerializeField] private GameObject gameOverScreen = null;
    public string mainMenuPath;
    public string gameScenePath;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (player != null) player.die += PlayerDie;
        gameOverScreen.SetActive(false);

    }

    //Initializes the game for each run.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void initRun()
    {
      
    }

    IEnumerator SetTimer(float t)
    {
        yield return new WaitForSeconds(t);
    }

    void PlayerDie(bool isDie)
    {
        gameOverScreen.SetActive(true);
        SetTimer(2f);
        LoadLevel(mainMenuPath);
    }

    void LoadLevel(string path)
    {          
        SceneManager.LoadScene(path);
    }
    

}