using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeakerSearchablePopupWindow : EditorWindow
{
    string search;
    Vector2 scrollpos = new Vector2();

    SpeakerTagContainer speakerTagContainer;
    Action<string> func;

    public static void Open(Action<string> _func)
    {
        var window = EditorWindow.CreateInstance<SpeakerSearchablePopupWindow>();

        window.func = _func;
        window.speakerTagContainer = AssetDatabase.LoadAssetAtPath<SpeakerTagContainer>("Assets/DyConv/Container/SpeakerTagContainer.asset")
            ?? throw new NullReferenceException("SpeakerTagContainer or its parent file does not exist");

        var mousePosition = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePosition.x, mousePosition.y, 0, 0), new Vector2(300, 200));
    }

    private void OnGUI()
    {
        search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField);

        string[] showElements = speakerTagContainer.tags.ToArray();

        if (!string.IsNullOrEmpty(search))
        {
            var sortedElements =
                from ele in showElements
                where ele.Contains(search)
                select ele;

            showElements = sortedElements.ToArray();
        }

        scrollpos = EditorGUILayout.BeginScrollView(scrollpos, GUI.skin.box);
        for (int i = 0; i < showElements.Length; i++)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(showElements[i], buttonStyle))
            {
                func.Invoke(showElements[i]);
                this.Close();
            }
            if(GUILayout.Button("-", GUILayout.Width(20)))
            {
                speakerTagContainer.tags.RemoveAt(i);
                EditorUtility.SetDirty(speakerTagContainer);
            }
            EditorGUILayout.EndHorizontal();
        }
        bool isDisabled = false;

        if (string.IsNullOrEmpty(search))
            isDisabled = true;

        if (showElements.Length > 0)
            isDisabled = true;

        EditorGUI.BeginDisabledGroup(isDisabled);
        if (GUILayout.Button("Add..", EditorStyles.foldoutHeader))
        {
            speakerTagContainer.tags.Add(search);
            EditorUtility.SetDirty(speakerTagContainer);

            func.Invoke(search);
            this.Close();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();
    }
}
