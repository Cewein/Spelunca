using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    [SerializeField]
    private ResourcesStock resourcesStock;


    void Awake()
    {
        DebugResourcesStockNotLoading();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator DebugResourcesStockNotLoading()
    {
        yield return new WaitForEndOfFrame();
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
      //  initRun();
    }

    //Initializes the game for each run.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void initRun()
    {
       // do global var initialisation for the current run.

       if (ResourcesStock.Instance == null)
       {
           resourcesStock = ScriptableObject.CreateInstance<ResourcesStock>();
       }

    }

}