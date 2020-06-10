﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class RingMenu : MonoBehaviour
{
    [Header("Back-end")]
    [SerializeField] private GunMagazine magazine;
    [Header("Images")]
    [SerializeField] private Sprite ring;
    [SerializeField] private GameObject element;

    [Header("Content")]
    [SerializeField] private string content = "Resources";
    
    [Header("Format")]
    [SerializeField] private float gap = 0f;
    [SerializeField] private float iconDist = 300f;
    [SerializeField] private Vector3 normalScale;
    [SerializeField] private Vector3 emphaseScale;

    private float stepLength;
    private int activeElement;
    private float NormalizeAngle(float a) => (a + 360f) % 360f;
    private GameObject[] items;
    private int contentLength;

    private void Awake()
    {
        Inventory.Instance.openResourceMenu += Open;
        Inventory.Instance.closeResourceMenu += Close;
        contentLength = (content == "Resources") ? ItemDataBase.Instance.resources.Count : 3;
        stepLength = 360f/contentLength;
        GetComponentInChildren<Image>(true).sprite = ring;
        items = new GameObject[contentLength];
        if (normalScale.Equals(Vector3.zero)) normalScale = transform.localScale;
        if (emphaseScale.Equals(Vector3.zero)) emphaseScale = transform.localScale*.7f;
    }

    private void Update()
    {
        if (items[0] == null)
        {
            for (int i = 0; i < contentLength; i++)
            {
                GameObject item = Instantiate(element,   transform.GetChild(0).transform);
                items[i] = item;
                item.GetComponent<Image>().sprite = (content == "Resources") ? ItemDataBase.Instance.resources[i].Icon : ItemDataBase.Instance.artifacts[i].Icon;
                
                item.transform.localPosition = Vector3.zero;
                item.transform.localPosition = item.transform.localPosition
                                               + Quaternion.AngleAxis(i * stepLength, Vector3.forward)
                                               * Vector3.up * iconDist;
                item.transform.localRotation = Quaternion.Euler(0, 0, 0);
           //     item.transform.localRotation = Quaternion.Euler(0, 0, gap + i*stepLength);
            }
        }
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) + stepLength / 2f);
        activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.localScale = (i == activeElement)  ? emphaseScale : normalScale;
        }
    }

  
    public void Open()
    {
        Cursor.lockState = CursorLockMode.None;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Close()
    {       
        transform.GetChild(0).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        // callback
        if (content == "Resources")
        {
            magazine.CurrentResource = ItemDataBase.Instance.resources[activeElement];
        }
    }
}
