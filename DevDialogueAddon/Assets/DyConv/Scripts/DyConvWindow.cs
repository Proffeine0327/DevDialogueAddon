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
    int currentSelectCriteriaDialogueIndex = -1;
    int currentSelectCriteriaIndex = -1;
    int currentSelectModificaionDialogueIndex = -1;
    int currentSelectModificaionIndex = -1;
    List<bool> shownNextDialogueList = new List<bool>();
    List<bool> shownCriteriaList = new List<bool>();
    List<bool> shownModificaionList = new List<bool>();
    Vector2 criteriaScrollPos = new Vector2();
    Vector2 modificaionScrollPos = new Vector2();

    [MenuItem("DyConv/Open Window")]
    public static void Open()
    {
        var window = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        window.Show();
    }

    [MenuItem("DyConv/test")]
    public static void test()
    {
        Dialogue.GetDialogue("test").nextDialogueInfos.Add(new NextDialogue());
        Dialogue.GetDialogue("test").nextDialogueInfos[0].criteriaKeys.Add("test");
        Dialogue.GetDialogue("test").nextDialogueInfos[0].compareTypes.Add(CompareType.Equal);
        Dialogue.GetDialogue("test").nextDialogueInfos[0].criteriaValues.Add(0);
        EditorUtility.SetDirty(Dialogue.GetDialogue("test"));
        AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue("test"));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();

        //Fact 이름과 값을 표시해주는 창
        #region Fact
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));

        EditorGUILayout.BeginHorizontal();
        var titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13, fontStyle = FontStyle.Bold };
        EditorGUILayout.LabelField("Facts", titleStyle);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            if (!Application.isPlaying)
                AddNewFactWindow.Open();
        }
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(currentFactName));
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            if (!string.IsNullOrEmpty(currentFactName) && !Application.isPlaying)
            {
                FactContainer.FactDictionary.Remove(currentFactName);

                EditorUtility.SetDirty(FactContainer.container);
                AssetDatabase.SaveAssetIfDirty(FactContainer.container);

                currentFactName = null;
                Dialogue.UpdateDialogue();
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // 여기에 나타남
        factScrollPos = EditorGUILayout.BeginScrollView(factScrollPos, EditorStyles.helpBox);
        for (int i = 0; i < FactContainer.Keys.Count; i++)
        {
            var factHorizontalStyle = new GUIStyle();
            factHorizontalStyle.normal.background = (FactContainer.Keys[i] == currentFactName ? Texture2D.grayTexture : Texture2D.blackTexture);
            EditorGUILayout.BeginHorizontal(factHorizontalStyle);

            //누르면 현재 Fact를 선택하고, Dialogue 선택과 Viewer 창을 초기화
            var buttonStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };
            if (GUILayout.Button(FactContainer.Keys[i], buttonStyle))
            {
                currentFactName = FactContainer.Keys[i];
                isActiveViewer = false;
                currentDialogueName = null;
                currentSelectCriteriaIndex = -1;
            }

            var textFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleRight };

            //변수 값 바꾸는 TextField
            EditorGUI.BeginChangeCheck();
            if (Application.isPlaying)
                DyConvManager.FactDictionary[FactContainer.Keys[i]] = EditorGUILayout.DelayedIntField(DyConvManager.FactDictionary[FactContainer.Keys[i]], textFieldStyle, GUILayout.MaxWidth(125));
            else
                FactContainer.FactDictionary[FactContainer.Keys[i]] = EditorGUILayout.DelayedIntField(FactContainer.Values[i], textFieldStyle, GUILayout.MaxWidth(125));
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(FactContainer.container);
                    AssetDatabase.SaveAssetIfDirty(FactContainer.container);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion

        //Dialogue 선택창을 나타냄
        #region Dialogue
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogues", titleStyle);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            if (!Application.isPlaying)
                AddNewDialogueWindow.Open();
        }

        EditorGUI.BeginDisabledGroup(!isActiveViewer || currentDialogueName == null);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            if (!Application.isPlaying)
            {
                AssetDatabase.DeleteAsset($"Assets/DyConv/Container/Dialogues/{currentDialogueName}.asset");
                currentDialogueName = null;
                isActiveViewer = false;

                AssetDatabase.SaveAssets();
                Dialogue.UpdateDialogue();
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        //여기에 나타남
        dialogueScrollPos = EditorGUILayout.BeginScrollView(dialogueScrollPos, EditorStyles.helpBox);

        for (int i = 0; i < Dialogue.allDialogues.Count; i++)
        {
            var buttonStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };
            buttonStyle.normal.background = (Dialogue.allDialogues[i].name == currentDialogueName ? Texture2D.grayTexture : Texture2D.blackTexture);

            //누르면 currentDialogueName 변수를 업데이트하고 FactIndex를 초기화시킴
            if (GUILayout.Button($" {Dialogue.allDialogues[i].name}", buttonStyle))
            {
                currentDialogueName = Dialogue.allDialogues[i].name;
                isActiveViewer = true;
                currentFactName = null;

                currentNextDialogueIndex = -1;
                currentSelectCriteriaIndex = -1;
                EditorGUI.FocusTextInControl(null);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion
        EditorGUILayout.EndVertical();

        //현재 Dialogue 상태를 나타냄
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
        //Dialogue 내용
        #region Text Field
        EditorGUILayout.LabelField("Text", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13 });

        EditorGUI.BeginChangeCheck();
        var textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fixedHeight = 60, fixedWidth = Screen.width * 0.65f - 15 };
        Dialogue.GetDialogue(currentDialogueName).dialogueText = EditorGUILayout.TextArea(Dialogue.GetDialogue(currentDialogueName).dialogueText, textAreaStyle);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));

        EditorGUILayout.Space(20);

        #endregion

        //다음 Dialogue로 이동할 수 있는 창(모든 Dialogue 경로를 탐색할 경우, Dialogue 끝으로 간주);
        #region Next Dialogues
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Next Dialogues");
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Add(new NextDialogue() { nextDialogueName = Dialogue.allDialogues[0].name });
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
            AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

            shownNextDialogueList.Add(new bool());
        }

        EditorGUI.BeginDisabledGroup(currentNextDialogueIndex == -1);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.RemoveAt(currentNextDialogueIndex);
            EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
            AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

            currentNextDialogueIndex = -1;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        while (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count != shownNextDialogueList.Count)
        {
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count < shownNextDialogueList.Count)
                shownNextDialogueList.RemoveAt(shownNextDialogueList.Count - 1);
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count > shownNextDialogueList.Count)
                shownNextDialogueList.Add(new bool());
        }

        while (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count != shownCriteriaList.Count)
        {
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count < shownCriteriaList.Count)
                shownCriteriaList.RemoveAt(shownCriteriaList.Count - 1);
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count > shownCriteriaList.Count)
                shownCriteriaList.Add(new bool());
        }

        while (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count != shownModificaionList.Count)
        {
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count < shownModificaionList.Count)
                shownModificaionList.RemoveAt(shownModificaionList.Count - 1);
            if (Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count > shownModificaionList.Count)
                shownModificaionList.Add(new bool());
        }

        //NextDialogue 수만큼 필드 생성
        for (int i = 0; i < Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos.Count; i++)
        {
            int nextDialogueIndex = i;
            var nextDialogue = Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[nextDialogueIndex];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var nextDialogueHorizontalStyle = new GUIStyle() { fixedHeight = 25 };
            nextDialogueHorizontalStyle.normal.background = (currentNextDialogueIndex == nextDialogueIndex ? Texture2D.grayTexture : Texture2D.blackTexture);

            //이동하게 될 Dialogue경로와 바꿀수 있는 버튼
            EditorGUILayout.BeginHorizontal(nextDialogueHorizontalStyle);
            shownNextDialogueList[i] = EditorGUILayout.Foldout(shownNextDialogueList[i], nextDialogue.nextDialogueName,
                 new GUIStyle(EditorStyles.foldout) { fixedWidth = 70, fixedHeight = 25, fontSize = 13 });

            //현재 NextDialogue를 선택
            if (GUILayout.Button("", GUI.skin.label, GUILayout.Height(25)))
            {
                currentNextDialogueIndex = i;
                currentSelectCriteriaIndex = -1;
            }

            //NextDialogue의 경로 변경
            if (GUILayout.Button(new GUIContent($" Select", EditorGUIUtility.IconContent("d_RotateTool On").image), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(100)))
            {
                DialogueSerachPopupWindow.Open((x) =>
                {
                    nextDialogue.nextDialogueName = Dialogue.GetDialogue(x).name;
                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                });
            }
            EditorGUILayout.EndHorizontal();

            if (shownNextDialogueList[nextDialogueIndex])
            {
                //Criteria 보여주기
                #region Criteria
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);

                EditorGUILayout.BeginHorizontal();
                shownCriteriaList[nextDialogueIndex] = EditorGUILayout.Foldout(shownCriteriaList[nextDialogueIndex], "Criteria");

                EditorGUI.BeginDisabledGroup(!shownCriteriaList[i]);
                EditorGUI.BeginDisabledGroup(FactContainer.FactDictionary.Count <= 0);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.criteriaKeys.Add(FactContainer.Keys[0]);
                    nextDialogue.compareTypes.Add(CompareType.Equal);
                    nextDialogue.criteriaValues.Add(0);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    shownCriteriaList.Add(new bool());
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(currentSelectCriteriaDialogueIndex != nextDialogueIndex || currentSelectCriteriaIndex == -1);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.criteriaKeys.RemoveAt(currentSelectCriteriaIndex);
                    nextDialogue.compareTypes.RemoveAt(currentSelectCriteriaIndex);
                    nextDialogue.criteriaValues.RemoveAt(currentSelectCriteriaIndex);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    currentSelectCriteriaIndex = -1;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                //만약 i번째 Criteria가 true라면 보여주기
                if (shownCriteriaList[nextDialogueIndex])
                {
                    criteriaScrollPos = EditorGUILayout.BeginScrollView(criteriaScrollPos, GUILayout.Height(95));
                    for (int j = 0; j < Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].criteriaKeys.Count; j++)
                    {
                        int criteriaIndex = j;

                        var criteriaStyle = new GUIStyle(EditorStyles.helpBox);
                        criteriaStyle.normal.background = currentSelectCriteriaDialogueIndex == nextDialogueIndex && currentSelectCriteriaIndex == criteriaIndex ? Texture2D.grayTexture : Texture2D.blackTexture;

                        EditorGUILayout.BeginHorizontal(criteriaStyle);

                        //클릭하면 검사 Fact 교체
                        if (GUILayout.Button(nextDialogue.criteriaKeys[criteriaIndex], new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
                        {
                            FactSearchablePopupWindow.Open((x) =>
                            {
                                nextDialogue.criteriaKeys[criteriaIndex] = x;
                                EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                                AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                            });
                        }

                        //같다, 다르다, 작다, 크다
                        string[] option = new string[] { "==", "!=", "<", ">" };
                        Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].compareTypes[j] = (CompareType)EditorGUILayout.Popup((int)Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].compareTypes[j], option,
                            new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(40));

                        //검사할 값
                        Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].criteriaValues[j] = EditorGUILayout.IntField(Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].criteriaValues[j],
                            new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(175));

                        //현재 Criteria index 선택
                        if (GUILayout.Button("☰", EditorStyles.label, GUILayout.MaxWidth(20)))
                        {
                            currentSelectCriteriaDialogueIndex = nextDialogueIndex;
                            currentSelectCriteriaIndex = criteriaIndex;

                            currentSelectModificaionIndex = -1;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                }
                EditorGUILayout.EndVertical();
                #endregion

                //Modificaion 보여주기
                #region Modification
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);

                EditorGUILayout.BeginHorizontal();
                shownModificaionList[nextDialogueIndex] = EditorGUILayout.Foldout(shownModificaionList[nextDialogueIndex], "Modificaion");

                EditorGUI.BeginDisabledGroup(!shownModificaionList[i]);
                EditorGUI.BeginDisabledGroup(FactContainer.FactDictionary.Count <= 0);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.modifyKeys.Add(FactContainer.Keys[0]);
                    nextDialogue.modifyValues.Add(0);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    shownModificaionList.Add(new bool());
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(currentSelectModificaionDialogueIndex != nextDialogueIndex || currentSelectModificaionIndex == -1);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.modifyKeys.RemoveAt(currentSelectModificaionIndex);
                    nextDialogue.modifyValues.RemoveAt(currentSelectModificaionIndex);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    currentSelectModificaionIndex = -1;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                //만약 i번째 Modificaion이 true면 표시
                if (shownModificaionList[nextDialogueIndex])
                {
                    modificaionScrollPos = EditorGUILayout.BeginScrollView(modificaionScrollPos, GUILayout.Height(95));
                    for (int j = 0; j < Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].modifyKeys.Count; j++)
                    {
                        int modificaionIndex = j;

                        var modificaionStyle = new GUIStyle(EditorStyles.helpBox);
                        modificaionStyle.normal.background = currentSelectModificaionDialogueIndex == nextDialogueIndex && currentSelectModificaionIndex == modificaionIndex ? Texture2D.grayTexture : Texture2D.blackTexture;

                        EditorGUILayout.BeginHorizontal(modificaionStyle);

                        //클릭하면 수정 Fact 교체
                        if (GUILayout.Button(nextDialogue.modifyKeys[modificaionIndex], new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }))
                        {
                            FactSearchablePopupWindow.Open((x) =>
                            {
                                nextDialogue.modifyKeys[modificaionIndex] = x;
                                EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                                AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                            });
                        }

                        //수정할 값
                        Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].modifyValues[j] = EditorGUILayout.IntField(Dialogue.GetDialogue(currentDialogueName).nextDialogueInfos[i].modifyValues[j],
                            new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(175));

                        //현재 Modificaion index 선택
                        if (GUILayout.Button("☰", EditorStyles.label, GUILayout.MaxWidth(20)))
                        {
                            currentSelectModificaionDialogueIndex = nextDialogueIndex;
                            currentSelectModificaionIndex = modificaionIndex;

                            currentSelectCriteriaIndex = -1;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
                #endregion
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion
    }
}
