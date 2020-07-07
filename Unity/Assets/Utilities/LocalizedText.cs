using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour{
    
    [LocalizedWord]
    public int text;

    void Start()
    {
        GetComponent<Text>().text = LocalizationSystem.Instance.Translate(LocalizationSystem.Instance.currentLanguage, text);
    }

    private void LateUpdate()
    {
        GetComponent<Text>().text = LocalizationSystem.Instance.Translate(LocalizationSystem.Instance.currentLanguage, text);

    }
}
