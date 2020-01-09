using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        initRun();
    }

    //Initializes the game for each run.
    void initRun()
    {
       // do global var initialisation for the current run.
    }

}