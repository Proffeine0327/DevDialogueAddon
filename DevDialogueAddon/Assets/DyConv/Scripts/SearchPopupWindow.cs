using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public enum SearchType
{
    Speaker,
    Fact,
    Funtion,
}

public class SearchPopupWindow : EditorWindow
{
    Speakers speakers;

    string searchText;
    SearchType searchType;

    List<string> elements = new List<string>();
    Action<string> selectAction;

    Vector2 scrollPos = new Vector2();

    public static void Open(SearchType _searchType, Action<string> func)
    {
        var speakerPopupWindow = EditorWindow.CreateInstance<SearchPopupWindow>();
        speakerPopupWindow.ShowAsDropDown(new Rect(EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition), Vector2.zero),
            new Vector2(300, 150));

        speakerPopupWindow.selectAction = func;
        speakerPopupWindow.speakers = AssetDatabase.LoadAssetAtPath<Speakers>("Assets/DyConv/Container/Speakers.asset");
        speakerPopupWindow.elements = speakerPopupWindow.speakers.speakerTags.ToList();
    }

    private void OnGUI()
    {
        var style = new GUIStyle(EditorStyles.toolbarSearchField);
        style.fontSize = 12;
        style.fixedHeight = 18;

        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        searchText = EditorGUILayout.TextField(searchText, style);

        List<string> showElements = new List<string>();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            showElements = elements;
        }
        else
        {
            var sortedElements =
                from ele in elements
                where ele.Contains(searchText)
                select ele;
            showElements = sortedElements.ToList();
        }

        EditorGUILayout.Space(8);

        GUI.skin.button.fontSize = 12;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.objectFieldThumb);

        if (showElements.Count() > 0)
            for (int i = 0; i < showElements.Count(); i++)
            {
                if (i % 2 == 0)
                    GUI.backgroundColor = Color.white;
                else
                    GUI.backgroundColor = Color.grey;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(showElements[i]))
                {
                    selectAction?.Invoke(showElements[i]);
                    this.Close();
                }
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    speakers.speakerTags.RemoveAt(i);
                    EditorUtility.SetDirty(speakers);
                    AssetDatabase.SaveAssetIfDirty(speakers);
                    
                    elements.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        else
        {
            bool isDisable = false;
            if (string.IsNullOrWhiteSpace(searchText))
                isDisable = true;

            EditorGUI.BeginDisabledGroup(isDisable);
            if (GUILayout.Button("Create New Tag", EditorStyles.foldoutHeader))
            {
                speakers.speakerTags.Add(searchText);
                EditorUtility.SetDirty(speakers);
                AssetDatabase.SaveAssetIfDirty(speakers);

                selectAction.Invoke(searchText);
                this.Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.EndScrollView();
    }
}
