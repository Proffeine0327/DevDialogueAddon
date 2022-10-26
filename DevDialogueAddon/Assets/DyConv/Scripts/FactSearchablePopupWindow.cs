using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FactSearchablePopupWindow : EditorWindow
{
    string search;

    Action<string> action;

    public static void Open(Action<string> action)
    {
        var window = EditorWindow.CreateInstance<FactSearchablePopupWindow>();

        window.action = action;
        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(300, 200));
    }

    private void OnGUI()
    {
        search = EditorGUILayout.TextField(search, new GUIStyle(EditorStyles.toolbarSearchField) { alignment = TextAnchor.MiddleLeft });

        string[] showElements = FactContainer.Keys.ToArray();

        if (!string.IsNullOrEmpty(search))
        {
            var searchedElements =
                from fName in showElements
                where fName.Contains(fName)
                select fName;

            showElements = searchedElements.ToArray();
        }

        for (int i = 0; i < showElements.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.SetIconSize(new Vector2(20,20));
            if (GUILayout.Button(new GUIContent($"   {showElements[i]}", EditorGUIUtility.IconContent("d_SaveAs").image), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
            {
                action.Invoke(showElements[i]);
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
