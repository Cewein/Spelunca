﻿using System.Linq;
using UI.Menu.RingMenu;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [Header("Menus")]
    [Tooltip("The resource magazine selection menu.")][SerializeField]
    private RingMenu resourceLoaderSelectionMenu;

    [Header("Inputs")] 
    [Tooltip("The input to display resource magazine selection menu.")][SerializeField]
    private string resourceLoaderSelectionInput = "ResourceSelection";

    [Header("Objects linked")] [Tooltip("The gun magazine.")] [SerializeField]
    private GunLoader gunMagazine;
    public Resource[] list;

    private void Start()
    {
        resourceLoaderSelectionMenu.callback += type =>
        {
            gunMagazine.CurrentResource = (type == ResourceType.normal) ? gunMagazine.NormalResource : list.First(item => item.Type == type);
        };
    }



    void Update()
    {
        resourceLoaderSelectionMenu.SetActive(Input.GetButton("ResourceSelection"));
    }
}