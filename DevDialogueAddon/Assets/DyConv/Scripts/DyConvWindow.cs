using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DyConvWindow : EditorWindow
{
    Vector2 factScrollPos = new Vector2();
    string currentFactName = "";

    Vector2 dialogueScrollPos = new Vector2();

    Vector2 viewScrollPos = new Vector2();
    bool isActiveViewer = false;
    Dialogue currentDialogue;
    List<bool> shownNextDialogueList = new List<bool>();
    List<bool> shownCriteriaList = new List<bool>();
    Vector2 criteriaScrollPos = new Vector2();

    [MenuItem("DyConv/Open Window")]
    public static void Open()
    {
        var window = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        window.Show();
    }

    [MenuItem("DyConv/test")]
    public static void test()
    {
        Dialogue.GetDialogue("test").nextDialogues.Add(new NextDialogue());
        Dialogue.GetDialogue("test").nextDialogues[0].criteriaKeys.Add("test");
        Dialogue.GetDialogue("test").nextDialogues[0].compareTypes.Add(CompareType.Equal);
        Dialogue.GetDialogue("test").nextDialogues[0].criteriaValues.Add(0);
        EditorUtility.SetDirty(Dialogue.GetDialogue("test"));
        AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue("test"));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        #region Fact
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));

        EditorGUILayout.BeginHorizontal();
        var titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13, fontStyle = FontStyle.Bold };
        EditorGUILayout.LabelField("Facts", titleStyle);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            AddNewFactWindow.Open();
        }
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(currentFactName));
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            if (!string.IsNullOrEmpty(currentFactName))
            {
                FactContainer.FactDictionary.Remove(currentFactName);
                EditorUtility.SetDirty(FactContainer.container);
                AssetDatabase.SaveAssetIfDirty(FactContainer.container);

                currentFactName = null;
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        factScrollPos = EditorGUILayout.BeginScrollView(factScrollPos, EditorStyles.helpBox);
        for (int i = 0; i < FactContainer.Keys.Count; i++)
        {
            var factHorizontalStyle = new GUIStyle();
            factHorizontalStyle.normal.background = (FactContainer.Keys[i] == currentFactName ? Texture2D.grayTexture : Texture2D.blackTexture);
            EditorGUILayout.BeginHorizontal(factHorizontalStyle);

            var buttonStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };
            if (GUILayout.Button(FactContainer.Keys[i], buttonStyle))
            {
                currentFactName = FactContainer.Keys[i];
                isActiveViewer = false;
                currentDialogue = null;
            }

            var textFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleRight };
            EditorGUI.BeginChangeCheck();
            FactContainer.FactDictionary[FactContainer.Keys[i]] = EditorGUILayout.DelayedIntField(FactContainer.Values[i], textFieldStyle, GUILayout.MaxWidth(125));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(FactContainer.container);
                AssetDatabase.SaveAssetIfDirty(FactContainer.container);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion

        #region Dialogue
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogues", titleStyle);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            AddNewDialogueWindow.Open();
        }

        EditorGUI.BeginDisabledGroup(!isActiveViewer || currentDialogue == null);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            AssetDatabase.DeleteAsset($"Assets/DyConv/Container/Dialogues/{currentDialogue.name}.asset");
            currentDialogue = null;
            isActiveViewer = false;

            AssetDatabase.SaveAssets();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        dialogueScrollPos = EditorGUILayout.BeginScrollView(dialogueScrollPos, EditorStyles.helpBox);

        for (int i = 0; i < Dialogue.allDialogues.Count; i++)
        {
            var buttonStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft , fontSize = 13};
            buttonStyle.normal.background = (Dialogue.allDialogues[i].name == currentDialogue?.name ? Texture2D.grayTexture : Texture2D.blackTexture);

            if (GUILayout.Button($" {Dialogue.allDialogues[i].name}", buttonStyle))
            {
                currentDialogue = Dialogue.allDialogues[i];
                isActiveViewer = true;
                currentFactName = null;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion
        EditorGUILayout.EndVertical();

        #region Viewer
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(Screen.width * 0.65f), GUILayout.MaxHeight(Screen.height));
        titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 15, fontStyle = FontStyle.Bold };

        EditorGUILayout.LabelField(currentDialogue != null && isActiveViewer ? $"Dialogue : {currentDialogue.name}" : "Viewer", titleStyle, GUILayout.Height(25));
        EditorGUILayout.Space(5);

        viewScrollPos = EditorGUILayout.BeginScrollView(viewScrollPos);
        if (currentDialogue != null && isActiveViewer)
            Viewer();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndHorizontal();
    }

    void Viewer()
    {
        EditorGUILayout.LabelField("Text", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 });

        EditorGUI.BeginChangeCheck();
        var textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fixedHeight = 60, fixedWidth = Screen.width * 0.65f - 15 };
        currentDialogue.dialogueText = EditorGUILayout.TextArea(currentDialogue.dialogueText, textAreaStyle);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(currentDialogue);

        EditorGUILayout.Space(20);

        while (currentDialogue.nextDialogues.Count != shownNextDialogueList.Count)
        {
            if (currentDialogue.nextDialogues.Count < shownNextDialogueList.Count)
                shownNextDialogueList.RemoveAt(shownNextDialogueList.Count - 1);
            if (currentDialogue.nextDialogues.Count > shownNextDialogueList.Count)
                shownNextDialogueList.Add(new bool());
        }

        while (currentDialogue.nextDialogues.Count != shownCriteriaList.Count)
        {
            if (currentDialogue.nextDialogues.Count < shownCriteriaList.Count)
                shownCriteriaList.RemoveAt(shownCriteriaList.Count - 1);
            if (currentDialogue.nextDialogues.Count > shownCriteriaList.Count)
                shownCriteriaList.Add(new bool());
        }

        for (int i = 0; i < currentDialogue.nextDialogues.Count; i++)
        {
            int index = i;
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            shownNextDialogueList[i] = EditorGUILayout.Foldout(shownNextDialogueList[i], currentDialogue.nextDialogues[i].nextDialogue?.name);
            if (GUILayout.Button(new GUIContent($" Change", EditorGUIUtility.IconContent("d_RotateTool On").image), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(100)))
            {
                DialogueSerachPopupWindow.Open((x) =>
                {
                    currentDialogue.nextDialogues[index].nextDialogue = Dialogue.GetDialogue(x);
                    EditorUtility.SetDirty(currentDialogue);
                    AssetDatabase.SaveAssetIfDirty(currentDialogue);
                });
            }
            EditorGUILayout.EndHorizontal();

            if (shownNextDialogueList[i])
            {

                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                shownCriteriaList[i] = EditorGUILayout.Foldout(shownCriteriaList[i], "Criteria");
                if (shownCriteriaList[i])
                {
                    criteriaScrollPos = EditorGUILayout.BeginScrollView(criteriaScrollPos, GUILayout.Height(95));
                    for (int j = 0; j < currentDialogue.nextDialogues[i].criteriaKeys.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        if (GUILayout.Button(currentDialogue.nextDialogues[i].criteriaKeys[j], new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
                        {

                        }
                        string[] option = new string[] { "==", "!=", "<", ">" };
                        currentDialogue.nextDialogues[i].compareTypes[j] = (CompareType)EditorGUILayout.Popup((int)currentDialogue.nextDialogues[i].compareTypes[j], option,
                            new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(40));
                        currentDialogue.nextDialogues[i].criteriaValues[j] = EditorGUILayout.IntField(currentDialogue.nextDialogues[i].criteriaValues[j],
                            new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(175));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
