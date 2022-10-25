using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueSerachPopupWindow : EditorWindow
{
    string search;

    Action<string> action;

    public static void Open(Action<string> action)
    {
        var window = EditorWindow.CreateInstance<DialogueSerachPopupWindow>();

        window.action = action;
        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(300, 200));
    }

    private void OnGUI()
    {
        search = EditorGUILayout.TextField(search, new GUIStyle(EditorStyles.toolbarSearchField) { alignment = TextAnchor.MiddleLeft });

        var dialogueNames =
            from dia in Dialogue.allDialogues
            select dia.name;

        string[] showElements = dialogueNames.ToArray();

        if (!string.IsNullOrEmpty(search))
        {
            var searchedElements =
                from dName in dialogueNames
                where dName.Contains(dName)
                select dName;

            showElements = searchedElements.ToArray();
        }

        for (int i = 0; i < showElements.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.SetIconSize(new Vector2(20,20));
            if (GUILayout.Button(new GUIContent($"   {showElements[i]}", EditorGUIUtility.IconContent("ScriptableObject Icon").image), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
            {
                action.Invoke(showElements[i]);
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
