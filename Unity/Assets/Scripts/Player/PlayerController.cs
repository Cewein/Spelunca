using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action<bool> run;

    private void Update()
    {
        isRunning(Input.GetButtonDown("Run"));
    }

    private bool isRunning(bool isRunning)
    {
        run?.Invoke(isRunning);
        return isRunning;
    }

}
