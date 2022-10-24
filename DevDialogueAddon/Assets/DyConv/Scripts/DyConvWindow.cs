using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DyConvWindow : EditorWindow
{
    Vector2 factScrollPos = new Vector2();

    [MenuItem("DyConv/Open Window")]
    public static void Open()
    {
        var window = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        window.Show();
    }

    [MenuItem("DyConv/test")]
    public static void test()
    {
        FactContainer.FactDictionary.Add("test", new FactValue() {type = FactValueType.Boolen, value = 1});
        EditorUtility.SetDirty(FactContainer.container);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        #region Select
        EditorGUILayout.BeginVertical();
        factScrollPos = EditorGUILayout.BeginScrollView(factScrollPos, GUI.skin.box,GUILayout.MaxWidth(Screen.width / 2));
        for (int i = 0; i < FactContainer.FactDictionary.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            var labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

            string showValue = "";
            if (FactContainer.Values[i].type == FactValueType.Boolen)
                showValue = Convert.ToBoolean(FactContainer.Values[i].value).ToString();
            else
                showValue = FactContainer.Values[i].value.ToString();

            EditorGUILayout.LabelField(showValue, labelStyle, GUILayout.MaxWidth(100));

            var buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (GUILayout.Button(FactContainer.Keys[i], buttonStyle))
            {
                
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        
        EditorGUILayout.EndVertical();
        #endregion

        #region Viewer
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width / 2));
        EditorGUILayout.TextField("test");
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndHorizontal();
    }
}
