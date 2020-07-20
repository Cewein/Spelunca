using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Localization", menuName = "ScriptableObjects/Localization")]

public class LocalizationSystem : MonoBehaviour
{
    private static LocalizationSystem _singleton;
    public static LocalizationSystem Instance { get { return _singleton; } }

    public List<Translation> languages;

    public int currentLanguage;
    [HideInInspector] public int wordsCount;

    [Serializable]
    public class Translation
    {
        [SerializeField] private string language;
        [SerializeField] private List<string> words;

        public List<string> Words => words;

        public string Language => language;

    }

    private void Awake()
    {
        _singleton = this;
        wordsCount = languages[0].Words.Count;
    }

    public string Translate(int language, int word)
    {
        return languages[language].Words[word];
    }

}
