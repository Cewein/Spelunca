using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public void Start()
    {
        GameManager.Instance.startNewScore();
    }
}
