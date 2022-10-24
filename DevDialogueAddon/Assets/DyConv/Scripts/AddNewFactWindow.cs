using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddNewFactWindow : EditorWindow
{
    string key;
    FactValueType valueType;

    public static void Open()
    {
        var window = EditorWindow.CreateInstance<AddNewFactWindow>();
        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        mousePos.x -= 125;
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(250, 20));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        key = EditorGUILayout.TextField(key);
        valueType = (FactValueType)EditorGUILayout.EnumPopup(valueType, GUILayout.Width(60));
        
        bool isDisable = false;
        if(string.IsNullOrEmpty(key)) isDisable = true;
        if(FactContainer.Keys.Contains(key)) isDisable = true;

        EditorGUI.BeginDisabledGroup(isDisable);
        if(GUILayout.Button("Add", GUILayout.MaxWidth(50)))
        {
            FactContainer.FactDictionary.Add(key, new FactValue() { type = valueType });
            EditorUtility.SetDirty(FactContainer.container);
            AssetDatabase.SaveAssetIfDirty(FactContainer.container);
            this.Close();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
}
