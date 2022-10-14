using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

enum ShowType
{
    none,
    fact,
    dialogue,
}

public class DyConvWindow : EditorWindow
{
    Facts facts;
    Vector2 factsScrollPosition = new Vector2();
    FactValue currentFactValue;
    ShowType showType;

    [MenuItem("DyConv/Window")]
    public static void Open()
    {
        DyConvWindow dyConvWindow = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        dyConvWindow.Show();
    }

    private void OnEnable()
    {
        facts = AssetDatabase.LoadAssetAtPath<Facts>("Assets/DyConv/Container/Facts.asset");
    }

    private void OnGUI()
    {
        #region Base Window
        GUI.skin.box.fontSize = 13;
        GUI.skin.box.normal.textColor = Color.white;

        GUI.Box(new Rect(5, 10, 60, 25), "Facts");
        GUI.Box(new Rect(5 + (Screen.width * 0.20f), 10, 80, 25), "Dialogues");
        GUI.Box(new Rect(5 + (Screen.width * 0.40f), 10, 70, 25), "Viewer");

        GUI.Box(new Rect(5, 35, Screen.width * 0.2f - 10, Screen.height - 65), "");
        GUI.Box(new Rect(5 + (Screen.width * 0.20f), 35, Screen.width * 0.2f - 10, Screen.height - 65), "");
        GUI.Box(new Rect(5 + (Screen.width * 0.40f), 35, Screen.width * 0.6f - 10, Screen.height - 65), "");
        #endregion

        #region Fact
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        GUI.skin.button.fontSize = 13;

        factsScrollPosition = GUI.BeginScrollView(new Rect(5, 35, Screen.width * 0.2f - 10, Screen.height - 65),
            factsScrollPosition, new Rect(0, 0, Screen.width * 0.15f, 30 * facts.allFacts.Count + 120));
        int index = 0;
        for (index = 0; index < facts.allFacts.Count; index++)
        {
            if (GUI.Button(new Rect(5, 5 + (index * 25), Screen.width * 0.2f - 25, 25), facts.allFacts.keys[index]))
            {
                if (Event.current.button == 1)
                {
                    facts.allFacts.Remove(facts.allFacts.keys[index]);
                    if (currentFactValue == facts.allFacts.values[index])
                        currentFactValue = null;
                    showType = ShowType.none;

                    EditorUtility.SetDirty(facts);
                    AssetDatabase.SaveAssetIfDirty(facts);
                }
                else
                {
                    currentFactValue = facts.allFacts.values[index];
                    showType = ShowType.fact;
                }
            }
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            string showValue = "";
            if (index < facts.allFacts.values.Count)
            {
                if (facts.allFacts.values[index].type == FactValueType.Bool)
                    showValue = Convert.ToBoolean(facts.allFacts.values[index].value).ToString();
                else
                    showValue = facts.allFacts.values[index].value.ToString();
            }
            GUI.Label(new Rect(5, 5 + (index * 25), Screen.width * 0.2f - 30, 25), showValue);
        }
        if (GUI.Button(new Rect(5, 5 + (index * 25), Screen.width * 0.2f - 25, 25), "Add..", EditorStyles.foldout))
            AddFactWidow.Open();
        GUI.EndScrollView();
        #endregion

        #region Viewer

        switch(showType)
        {
            case ShowType.fact:
                ShowFact();
                break;
            
            case ShowType.dialogue:
                break;
            
            default:
                break;
        }

        #endregion
    }

    void ShowFact()
    {
        if (currentFactValue != null)
        {
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontSize = 13;
            GUI.Label(new Rect(15 + (Screen.width * 0.40f), 35, Screen.width * 0.6f - 20, 30), "Value");

            if (currentFactValue.type == FactValueType.Int)
            {
                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.DelayedIntField(new Rect(15 + (Screen.width * 0.40f), 65, 300, 20), (int)currentFactValue.value);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(facts);
                    AssetDatabase.SaveAssetIfDirty(facts);
                }
            }
            if (currentFactValue.type == FactValueType.Float)
            {
                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.DelayedFloatField(new Rect(15 + (Screen.width * 0.40f), 65, 300, 20), currentFactValue.value);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(facts);
                    AssetDatabase.SaveAssetIfDirty(facts);
                }
            }
            if (currentFactValue.type == FactValueType.Bool)
            {
                string[] options = new string[] { "False", "True" };

                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.Popup(new Rect(15 + (Screen.width * 0.40f), 65, 300, 20), (int)currentFactValue.value, options);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(facts);
                    AssetDatabase.SaveAssetIfDirty(facts);
                }
            }
        }
    }
}

public class AddFactWidow : EditorWindow
{
    Facts facts;
    string tagString;
    FactValueType type;

    public static void Open()
    {
        AddFactWidow addFactWidow = EditorWindow.CreateInstance<AddFactWidow>();
        addFactWidow.ShowAsDropDown(new Rect(EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition), Vector2.zero), new Vector2(250, 20));
        addFactWidow.facts = AssetDatabase.LoadAssetAtPath<Facts>("Assets/DyConv/Container/Facts.asset");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        tagString = EditorGUILayout.TextField(tagString);

        bool isDisable = false;

        if (string.IsNullOrWhiteSpace(tagString))
            isDisable = true;
        if (facts.allFacts.keys.Contains(tagString))
            isDisable = true;

        type = (FactValueType)EditorGUILayout.EnumPopup(type, GUILayout.Width(50));

        EditorGUI.BeginDisabledGroup(isDisable);
        if (GUILayout.Button("Add"))
        {
            FactValue v = new FactValue();
            v.type = type;
            facts.allFacts.Add(tagString, v);

            EditorUtility.SetDirty(facts);
            AssetDatabase.SaveAssetIfDirty(facts);
            this.Close();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
}
