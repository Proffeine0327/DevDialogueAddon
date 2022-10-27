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

        //Fact 이름과 값을 표시해주는 창
        #region Fact
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxHeight(Screen.height / 2));

        EditorGUILayout.BeginHorizontal();
        var titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 13, fontStyle = FontStyle.Bold };
        EditorGUILayout.LabelField("Facts", titleStyle);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
        {
            if(!Application.isPlaying)
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
            if(Application.isPlaying)
                DyConvManager.FactDictionary[FactContainer.Keys[i]] = EditorGUILayout.DelayedIntField(DyConvManager.FactDictionary[FactContainer.Keys[i]], textFieldStyle, GUILayout.MaxWidth(125));
            else
                FactContainer.FactDictionary[FactContainer.Keys[i]] = EditorGUILayout.DelayedIntField(FactContainer.Values[i], textFieldStyle, GUILayout.MaxWidth(125));
            if (EditorGUI.EndChangeCheck())
            {
                if(!Application.isPlaying)
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
            if(!Application.isPlaying)
                AddNewDialogueWindow.Open();
        }

        EditorGUI.BeginDisabledGroup(!isActiveViewer || currentDialogueName == null);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
        {
            if(!Application.isPlaying)
            {
                AssetDatabase.DeleteAsset($"Assets/DyConv/Container/Dialogues/{currentDialogueName}.asset");
                currentDialogueName = null;
                isActiveViewer = false;

                AssetDatabase.SaveAssets();
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

        EditorGUILayout.BeginVertical(GUI.skin.box);
        #endregion

        //다음 Dialogue로 이동할 수 있는 창(모든 Dialogue 경로를 탐색할 경우, Dialogue 끝으로 간주);
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

        //NextDialogue 수만큼 필드 생성
        for (int i = 0; i < Dialogue.GetDialogue(currentDialogueName).nextDialogues.Count; i++)
        {
            int nextDialogueIndex = i;
            var nextDialogue = Dialogue.GetDialogue(currentDialogueName).nextDialogues[nextDialogueIndex];

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var nextDialogueHorizontalStyle = new GUIStyle() { fixedHeight = 25 };
            nextDialogueHorizontalStyle.normal.background = (currentNextDialogueIndex == nextDialogueIndex ? Texture2D.grayTexture : Texture2D.blackTexture);

            //이동하게 될 Dialogue경로와 바꿀수 있는 버튼
            EditorGUILayout.BeginHorizontal(nextDialogueHorizontalStyle);
            shownNextDialogueList[i] = EditorGUILayout.Foldout(shownNextDialogueList[i], nextDialogue.NextDialogueName,
                 new GUIStyle(EditorStyles.foldout) { fixedWidth = 70,fixedHeight = 25 , fontSize = 13 });

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
                    nextDialogue.NextDialogueName = Dialogue.GetDialogue(x).name;
                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                });
            }
            EditorGUILayout.EndHorizontal();

            //Criteria 보여주기
            if (shownNextDialogueList[nextDialogueIndex])
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);

                EditorGUILayout.BeginHorizontal();
                shownCriteriaList[nextDialogueIndex] = EditorGUILayout.Foldout(shownCriteriaList[nextDialogueIndex], "Criteria");

                EditorGUI.BeginDisabledGroup(!shownCriteriaList[i]);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.criteriaKeys.Add("test");
                    nextDialogue.compareTypes.Add(CompareType.Equal);
                    nextDialogue.criteriaValues.Add(0);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));

                    shownCriteriaList.Add(new bool());
                }

                EditorGUI.BeginDisabledGroup(currentSelectCriteriaDialogueIndex != nextDialogueIndex || currentSelectCriteriaIndex == -1);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, EditorStyles.iconButton))
                {
                    nextDialogue.criteriaKeys.RemoveAt(currentSelectCriteriaIndex);
                    nextDialogue.compareTypes.RemoveAt(currentSelectCriteriaIndex);
                    nextDialogue.criteriaValues.RemoveAt(currentSelectCriteriaIndex);

                    EditorUtility.SetDirty(Dialogue.GetDialogue(currentDialogueName));
                    AssetDatabase.SaveAssetIfDirty(Dialogue.GetDialogue(currentDialogueName));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUI.EndDisabledGroup();

                //만약 i번째 Criteria가 true라면 보여주기
                if (shownCriteriaList[i])
                {
                    criteriaScrollPos = EditorGUILayout.BeginScrollView(criteriaScrollPos, GUILayout.Height(95));
                    for (int j = 0; j < Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaKeys.Count; j++)
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
                        Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].compareTypes[j] = (CompareType)EditorGUILayout.Popup((int)Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].compareTypes[j], option,
                            new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(40));
                        
                        //검사할 값
                        Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaValues[j] = EditorGUILayout.IntField(Dialogue.GetDialogue(currentDialogueName).nextDialogues[i].criteriaValues[j],
                            new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(175));

                        //현재 Criteria index 선택
                        if(GUILayout.Button("☰", EditorStyles.label,GUILayout.MaxWidth(20)))
                        {
                            currentSelectCriteriaDialogueIndex = nextDialogueIndex;
                            currentSelectCriteriaIndex = criteriaIndex;
                        }
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
