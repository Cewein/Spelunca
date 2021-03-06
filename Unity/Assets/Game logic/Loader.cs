﻿        using UnityEngine;

[RequireComponent(typeof(GameManager))]

/// <summary>
///  This loader instantiate persistent singletons at each scenes beginning.
/// </summary>
public class Loader : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }
}