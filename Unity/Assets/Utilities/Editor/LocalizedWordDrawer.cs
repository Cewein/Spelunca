using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(LocalizedWordAttribute))]

public class LocalizedWordDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            InputNameAttribute localizedWord = attribute as InputNameAttribute;
            Rect rectangle = EditorGUI.PrefixLabel(position, label);
            Rect dropdownMenuRect = new Rect(rectangle.x , rectangle.y, rectangle.width, rectangle.height);
            string[] inputNames = GetEnglishWord().ToArray();
            int currentIndex = property.intValue;
            property.intValue = EditorGUI.Popup(dropdownMenuRect, currentIndex, inputNames);
        }
        else GUI.Label(position, "LocalizedWord attribute can only work with interger !");
    }
    
    private List<string> GetEnglishWord()
    {
        return LocalizationSystem.Instance.Languages[0].Words;
    }

    private int getWordIndex(string w)
    {
        return LocalizationSystem.Instance.Languages[0].Words.IndexOf(w);
    }
    
    private int getIndex(string s, string[] tab)
    {
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].Equals(s)) return i;
        }
        return 0;
    }

}
