using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Localization", menuName = "ScriptableObjects/Localization")]

public class LocalizationSystem : SingletonScriptableObject<LocalizationSystem>
{
    [SerializeField] private List<Translation> languages;

    public List<Translation> Languages => languages;

    public int currentLanguage;
    [HideInInspector] public int wordsCount;
    [Serializable]
    public class Translation
    {
        [SerializeField] private string language;
        [SerializeField] private  List<string>  words;

        public List<string> Words => words;

    }

    private void Awake()
    {
        wordsCount = languages[0].Words.Count;
    }

    public string Translate(int language ,int word)
    {
        return languages[language].Words[word];
    }

}
