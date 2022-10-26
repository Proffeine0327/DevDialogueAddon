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
    string currentDialogueName;
    int currentNextDialogueIndex = -1;
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
                currentDialogueName = null;
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

        EditorGUI.BeginDisabledGroup(!isActiveViewer || currentDialogueName == null);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            AssetDatabase.DeleteAsset($"Assets/DyConv/Container/Dialogues/{currentDialogueName}.asset");
            currentDialogueName = null;
            isActiveViewer = false;

            AssetDatabase.SaveAssets();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        dialogueScrollPos = EditorGUILayout.BeginScrollView(dialogueScrollPos, EditorStyles.helpBox);

        for (int i = 0; i < Dialogue.allDialogues.Count; i++)
        {
            var buttonStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };
            buttonStyle.normal.background = (Dialogue.allDialogues[i].name == currentDialogueName ? Texture2D.grayTexture : Texture2D.blackTexture);

            if (GUILayout.Button($" {Dialogue.allDialogues[i].name}", buttonStyle))
            {
                currentDialogueName = Dialogue.allDialogues[i].name;
                isActiveViewer = true;
                currentFactName = null;

                currentNextDialogueIndex = -1;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion
        EditorGUILayout.EndVertical();

        #region Viewer
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(Screen.width * 0.65f), GUILayout.MaxHeight(Screen.height));
        titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 15, fontStyle = FontStyle.Bold };

        EditorGUILayout.LabelField(currentDialogueName != null && isActiveViewer ? $"Dialogue : {currentDialogueName}" : "Viewer", titleStyle, GUILayout.Height(25));
        EditorGUILayout.Space(5);

        viewScrollPos = EditorGUILayout.BeginScrollView(viewScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);
        if (currentDialogueName != null && isActiveViewer)
            Viewer();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndHorizontal();
    }

    void Viewer()
    {
        #region Text Field
        EditorGUILayout.LabelField("Text", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 });

        EditorGUI.BeginChangeCheck();
        var textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fixedHeight = 60, fixedWidth = Screen.width * 0.65f - 15 };
        Dialogue.GetDialogue(currentDialogueName).dialogueText = EditorGUILayout.TextArea(Dialogue.GetDialogue(currentDialogueName).dialogueText, textAreaStyle);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        #endregion

        #region Next Dialogues
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Next Dialogue");

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            Dialogue.GetDialogue(currentDialogueName).nextDialogues.Add(new NextDialogue());
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
            AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

            shownNextDialogueList.Add(new bool());
        }

        EditorGUI.BeginDisabledGroup(currentNextDialogueIndex == -1);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            Dialogue.GetDialogue(currentDialogueName).nextDialogues.RemoveAt(currentNextDialogueIndex);
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
            AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

            currentNextDialogueIndex = -1;
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        while (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count != shownNextDialogueList.Count)
        {
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count < shownNextDialogueList.Count)
                shownNextDialogueList.RemoveAt(shownNextDialogueList.Count - 1);
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count > shownNextDialogueList.Count)
                shownNextDialogueList.Add(new bool());
        }

        while (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count != shownCriteriaList.Count)
        {
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count < shownCriteriaList.Count)
                shownCriteriaList.RemoveAt(shownCriteriaList.Count - 1);
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count > shownCriteriaList.Count)
                shownCriteriaList.Add(new bool());
        }

        for (int i = 0; i < Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count; i++)
        {
            int nextDialogueIndex = i;
            var nextDialogue = Dialogue.GetDialogue(currentDialogueName).nextDialogues[nextDialogueIndex];

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var nextDialogueHorizontalStyle = new GUIStyle() { fixedHeight = 25 };
            nextDialogueHorizontalStyle.normal.background = (currentNextDialogueIndex == nextDialogueIndex ? Texture2D.grayTexture : Texture2D.blackTexture);

            EditorGUILayout.BeginHorizontal(nextDialogueHorizontalStyle);
            shownNextDialogueList[i] = EditorGUILayout.Foldout(shownNextDialogueList[i], nextDialogue.NextDialogueName,
                 new GUIStyle(EditorStyles.foldout) { fixedWidth = 70,fixedHeight = 25 , fontSize = 13 });

            if (GUILayout.Button("", GUI.skin.label, GUILayout.Height(25)))
            {
                currentNextDialogueIndex = i;
            }

            if (GUILayout.Button(new GUIContent($" Select", EditorGUIUtility.IconContent("d_RotateTool On").image), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(100)))
            {
                DialogueSerachPopupWindow.Open((x) =>
                {
                    nextDialogue.NextDialogueName = Dialogue.GetDialogue(x).name;
                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                });
            }
            EditorGUILayout.EndHorizontal();

            if (shownNextDialogueList[nextDialogueIndex])
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);

                EditorGUILayout.BeginHorizontal();
                shownCriteriaList[nextDialogueIndex] = EditorGUILayout.Foldout(shownCriteriaList[nextDialogueIndex], "Criteria");
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.criteriaKeys.Add("test");
                    nextDialogue.compareTypes.Add(CompareType.Equal);
                    nextDialogue.criteriaValues.Add(0);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    shownCriteriaList.Add(new bool());
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
                {

                }
                EditorGUILayout.EndHorizontal();

                if (shownCriteriaList[i])
                {
                    criteriaScrollPos = EditorGUILayout.BeginScrollView(criteriaScrollPos, GUILayout.Height(95));
                    for (int j = 0; j < Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaKeys.Count; j++)
                    {
                        int criteriaIndex = j;
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        if (GUILayout.Button(nextDialogue.criteriaKeys[criteriaIndex], new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
                        {
                            FactSearchablePopupWindow.Open((x) =>
                            {
                                nextDialogue.criteriaKeys[criteriaIndex] = x;
                                EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                                AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                            });
                        }
                        string[] option = new string[] { "==", "!=", "<", ">" };
                        Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].compareTypes[j] = (CompareType)EditorGUILayout.Popup((int)Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].compareTypes[j], option,
                            new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(40));
                        Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaValues[j] = EditorGUILayout.IntField(Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaValues[j],
                            new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(175));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndVertical();
        #endregion
    }
}
