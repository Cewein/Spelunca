﻿using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChooseLanguage : MonoBehaviour
{
    private void Awake()
    {
        Refresh();
    }

    private void Refresh()
    {
        GetComponent<Text>().text = LocalizationSystem.Instance
                                    .languages[LocalizationSystem.Instance.currentLanguage]
                                    .Language;
    }

    public void OnClick()
    {
        LocalizationSystem.Instance.currentLanguage = (LocalizationSystem.Instance.currentLanguage + 1) %
                                                      LocalizationSystem.Instance.languages.Count;
        
        Refresh();
    }
}
