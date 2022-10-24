using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DyConvWindow : EditorWindow
{
    Vector2 factScrollPos = new Vector2();
    bool showFact;

    Vector2 dialogueScrollPos = new Vector2();
    bool showDialogue;

    Vector2 viewScrollPos = new Vector2();
    bool isShowDialogue = false;
    Dialogue currentDialogue;

    [MenuItem("DyConv/Open Window")]
    public static void Open()
    {
        var window = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        window.Show();
    }

    [MenuItem("DyConv/test")]
    public static void test()
    {
        FactContainer.FactDictionary.Add("test", new FactValue() { type = FactValueType.Boolen, value = 1 });
        EditorUtility.SetDirty(FactContainer.container);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        #region Fact
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));
        var titleStyle = new GUIStyle(EditorStyles.foldout) { alignment = TextAnchor.MiddleLeft, fontSize = 13, fontStyle = FontStyle.Bold };
        showFact = EditorGUILayout.Foldout(showFact, "Facts", titleStyle);
        if (showFact)
        {
            factScrollPos = EditorGUILayout.BeginScrollView(factScrollPos, EditorStyles.helpBox);
            for (int i = 0; i < FactContainer.Keys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
                EditorGUILayout.LabelField(FactContainer.Keys[i], buttonStyle);


                if (FactContainer.Values[i].type == FactValueType.Int)
                {
                    var textFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleRight };
                    int value = Convert.ToInt32(FactContainer.Values[i].value);
                    EditorGUI.BeginChangeCheck();
                    FactContainer.Values[i].value = EditorGUILayout.DelayedIntField(value, textFieldStyle, GUILayout.MaxWidth(125));
                    if (EditorGUI.EndChangeCheck())
                        EditorUtility.SetDirty(FactContainer.container);
                }
                else if (FactContainer.Values[i].type == FactValueType.Float)
                {
                    var textFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleRight };
                    EditorGUI.BeginChangeCheck();
                    FactContainer.Values[i].value = EditorGUILayout.DelayedFloatField(FactContainer.Values[i].value, textFieldStyle, GUILayout.MaxWidth(125));
                    if (EditorGUI.EndChangeCheck())
                        EditorUtility.SetDirty(FactContainer.container);
                }
                else if (FactContainer.Values[i].type == FactValueType.Boolen)
                {
                    var textFieldStyle = new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleRight };
                    EditorGUI.BeginChangeCheck();
                    string[] option = new string[] { "False", "True" };
                    FactContainer.Values[i].value = EditorGUILayout.Popup((int)FactContainer.Values[i].value, option, textFieldStyle, GUILayout.MaxWidth(125));
                }


                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                {
                    FactContainer.FactDictionary.Remove(FactContainer.Keys[i]);
                    EditorUtility.SetDirty(FactContainer.container);
                }
                EditorGUILayout.EndHorizontal();
            }
            AssetDatabase.SaveAssetIfDirty(FactContainer.container);
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add..", new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(125)))
            {
                AddNewFactWindow.Open();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Dialogue
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));
        showDialogue = EditorGUILayout.Foldout(showDialogue, "Dialogues", titleStyle);
        if (showDialogue)
        {
            dialogueScrollPos = EditorGUILayout.BeginScrollView(dialogueScrollPos, EditorStyles.helpBox);
            List<Dialogue> dialogues = new List<Dialogue>();
            string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
            foreach (var guid in guids) dialogues.Add(AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guid)));

            for (int i = 0; i < dialogues.Count; i++)
            {
                var buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
                if (GUILayout.Button(dialogues[i].name, buttonStyle))
                {

                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
        #endregion
        EditorGUILayout.EndVertical();

        #region Viewer
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(Screen.width * 0.65f), GUILayout.MaxHeight(Screen.height));
        titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 15, fontStyle = FontStyle.Bold };
        EditorGUILayout.LabelField("Viewer", titleStyle, GUILayout.Height(25));

        viewScrollPos = EditorGUILayout.BeginScrollView(viewScrollPos);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndHorizontal();
    }
}
