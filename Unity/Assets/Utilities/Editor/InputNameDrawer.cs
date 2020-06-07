using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Draws InputNameAttribute dropdown menu in inspector.
/// </summary>
[CustomPropertyDrawer(typeof(InputNameAttribute))]
public class InputNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            InputNameAttribute inputName = attribute as InputNameAttribute;
            Rect rectangle = EditorGUI.PrefixLabel(position, label);
            Rect dropdownMenuRect = new Rect(rectangle.x , rectangle.y, rectangle.width, rectangle.height);
            string[] inputNames = getInputNamesList().ToArray();
            int currentIndex = getIndex(property.stringValue, inputNames);
            property.stringValue = inputNames[EditorGUI.Popup(dropdownMenuRect, currentIndex, inputNames)];
        }
        else GUI.Label(position, "InputName attribute can only work with string !");
    }

    /// <summary>
    /// Return a list of strings that contains all input names defined in Project Settings.
    /// </summary>
    private List<string> getInputNamesList()
    {
        var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        SerializedObject obj = new SerializedObject(inputManager);
        SerializedProperty axisArray = obj.FindProperty("m_Axes");
        List<string> inputNames = new List<string>();

        if (axisArray.arraySize == 0) inputNames.Add("No input is defined in project settings.");
        else
        {
            for (int i = 0; i < axisArray.arraySize; i++)
            {
                var axe = axisArray.GetArrayElementAtIndex(i);
                inputNames.Add(axe.FindPropertyRelative("m_Name").stringValue);
            }
        }
        return inputNames;
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