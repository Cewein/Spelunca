using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ResourcesStock resourcesStock;

    public static GameManager instance = null;
    [SerializeField] private PlayerStats player = null;
    [SerializeField] private GameObject gameOverScreen = null;
    public string mainMenuPath;
    public string gameScenePath;

    void Awake()
    {
        DebugResourcesStockNotLoading();

        if (player != null) player.die += PlayerDie;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

    }
    private IEnumerator DebugResourcesStockNotLoading()
    {
        yield return new WaitForEndOfFrame();
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        //  initRun();
    }

    void PlayerDie(bool isDie)
    {
        gameOverScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadLevel(mainMenuPath);
    }

    void LoadLevel(string path)
    {          
        SceneManager.LoadScene(path);
    }

    public void StartNewGame()
    {
        LoadLevel(gameScenePath);
    }
}