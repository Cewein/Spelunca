﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputNameAttribute))]
public class InputNameDrawer : PropertyDrawer
{
    private int index;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InputNameAttribute inputNameAttribute = attribute as InputNameAttribute;
        if (!property.propertyType.Equals(SerializedPropertyType.String))
        {
            EditorGUI.LabelField(position, label.text, "Input name attribute only work with string type.");
        }
        else
        {
            string[] list = GetInputManagerAxisList();
            index  = EditorGUI.Popup(position, property.displayName, Array.IndexOf(list, property.stringValue), list);
            property.stringValue = list[index];
            
        }
    
    }

    private string[] GetInputManagerAxisList()
    {
        SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axis = inputManager.FindProperty("m_Axes");
        string[] inputNameList = new string[axis.arraySize];
        int i = 0;
        foreach (SerializedProperty axe in axis)
        {
            inputNameList[i++] = axe.FindPropertyRelative("m_Name").stringValue;
        }

        return inputNameList;
    }

    private string ValidateInputName(string name)
    {
        return GetInputManagerAxisList().Contains(name) ? name : GetInputManagerAxisList()[0];
    }
}