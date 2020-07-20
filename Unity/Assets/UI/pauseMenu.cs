using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
{

    public GameObject mainSreen;
    public GameObject pauseScreen;

    bool inPause;

    private void Start()
    {
        inPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            mainSreen.SetActive(inPause);
            pauseScreen.SetActive(!inPause);
            inPause = !inPause;
            Cursor.visible = inPause;
            Cursor.lockState = inPause ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
}
