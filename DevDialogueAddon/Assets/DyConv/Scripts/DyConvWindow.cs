using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DyConvWindow : EditorWindow
{
    Facts facts;
    List<Dialogue> dialogues = new List<Dialogue>();
    
    Vector2 factsScrollPosition = new Vector2();
    Vector2 dialoguesScrollPosition = new Vector2();
    Vector2 viewerScrollPosition = new Vector2();

    FactValue currentFactValue;
    string currentFactName;

    Dialogue currentDialogueValue;
    string dialogueText;

    int showType; // 0 == none, 1 == fact, 2 == dialogue

    [MenuItem("DyConv/Window")]
    public static void Open()
    {
        DyConvWindow dyConvWindow = EditorWindow.GetWindow<DyConvWindow>("DyConv");
        dyConvWindow.Show();
    }

    [MenuItem("DyConv/test")]
    public static void test()
    {
        Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>("Assets/DyConv/Container/Dialogues/test.asset");
        dialogue.nextDialogues.Add(new NextDialogue());
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
            factsScrollPosition, new Rect(0, 0, Screen.width * 0.15f, 30 * facts.allFacts.Count + 90));
        int factRectIndex = 0;
        for (factRectIndex = 0; factRectIndex < facts.allFacts.Count; factRectIndex++)
        {
            if (GUI.Button(new Rect(5, 5 + (factRectIndex * 25), Screen.width * 0.2f - 25, 25), facts.allFacts.keys[factRectIndex]))
            {
                if (Event.current.button == 1)
                {
                    facts.allFacts.Remove(facts.allFacts.keys[factRectIndex]);
                    if (currentFactValue == facts.allFacts.values[factRectIndex])
                        currentFactValue = null;
                    showType = 0;

                    EditorUtility.SetDirty(facts);
                    AssetDatabase.SaveAssetIfDirty(facts);
                }
                else
                {
                    currentFactValue = facts.allFacts.values[factRectIndex];
                    currentFactName = facts.allFacts.keys[factRectIndex];
                    showType = 1;
                }
            }
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            string showValue = "";
            if (factRectIndex < facts.allFacts.values.Count)
            {
                if (facts.allFacts.values[factRectIndex].type == FactValueType.Bool)
                    showValue = Convert.ToBoolean(facts.allFacts.values[factRectIndex].value).ToString();
                else
                    showValue = facts.allFacts.values[factRectIndex].value.ToString();
            }
            GUI.Label(new Rect(5, 5 + (factRectIndex * 25), Screen.width * 0.2f - 30, 25), showValue);
        }
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        if (GUI.Button(new Rect(Screen.width * 0.1f - 100, 10 + (factRectIndex * 25), 190, 25), "Add New Fact"))
            AddFactWindow.Open();
        GUI.EndScrollView();
        #endregion

        #region Dialogue

        dialogues.Clear();
        string[] dialogueGuids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
        for (int i = 0; i < dialogueGuids.Length; i++) dialogues.Add(AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(dialogueGuids[i])));

        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        dialoguesScrollPosition = GUI.BeginScrollView(new Rect(5 + (Screen.width * 0.20f), 35, Screen.width * 0.2f - 10, Screen.height - 65),
            dialoguesScrollPosition, new Rect(0, 0, Screen.width * 0.15f, dialogues.Count + 90));
        int dialogueRectIndex = 0;
        for (dialogueRectIndex = 0; dialogueRectIndex < dialogues.Count; dialogueRectIndex++)
        {
            if (GUI.Button(new Rect(5, 5 + (dialogueRectIndex * 25), Screen.width * 0.2f - 20, 25), dialogues[dialogueRectIndex].name))
            {
                if (Event.current.button == 1)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(dialogues[dialogueRectIndex]));
                    AssetDatabase.SaveAssets();

                    if(currentDialogueValue == dialogues[dialogueRectIndex])
                        currentDialogueValue = null;
                    
                    showType = 0;
                }
                else
                {
                    currentDialogueValue = dialogues[dialogueRectIndex];
                    dialogueText = currentDialogueValue.dialogue;
                    showType = 2;
                }
            }
        }

        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        if (GUI.Button(new Rect(Screen.width * 0.1f - 100, 10 + (dialogueRectIndex * 25), 190, 25), "Add New Dialogue"))
            AddDialogueWindow.Open();
        GUI.EndScrollView();
        #endregion

        #region Viewer
        switch (showType)
        {
            case 1:
                ShowFact();
                break;

            case 2:
                ShowDialogue();
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
            GUI.Label(new Rect(15 + (Screen.width * 0.40f), 35, Screen.width * 0.6f - 20, 30), currentFactName);

            GUI.Label(new Rect(15 + (Screen.width * 0.40f), 60, Screen.width * 0.6f - 20, 30), "Value");
            if (currentFactValue.type == FactValueType.Int)
            {
                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.IntField(new Rect(75 + (Screen.width * 0.40f), 65, 300, 20), (int)currentFactValue.value);
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(facts);
            }
            if (currentFactValue.type == FactValueType.Float)
            {
                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.FloatField(new Rect(75 + (Screen.width * 0.40f), 65, 300, 20), currentFactValue.value);
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(facts);
            }
            if (currentFactValue.type == FactValueType.Bool)
            {
                string[] options = new string[] { "False", "True" };

                EditorGUI.BeginChangeCheck();
                currentFactValue.value = EditorGUI.Popup(new Rect(75 + (Screen.width * 0.40f), 65, 300, 20), (int)currentFactValue.value, options);
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(facts);
            }
        }
    }

    void ShowDialogue()
    {
        if (currentDialogueValue != null)
        {
            var baseWidth = GUILayout.MaxWidth(Screen.width * 0.6f - 35);

            viewerScrollPosition = GUI.BeginScrollView(new Rect(10 + (Screen.width * 0.40f), 35, Screen.width * 0.6f - 15, Screen.height - 65), 
                viewerScrollPosition, new Rect(0,0,Screen.width * 0.1f,1000));
            
            var labelStyle = new GUIStyle(GUI.skin.label){ fontSize = 15, fixedWidth = Screen.width * 0.6f - 35};
            EditorGUILayout.LabelField(currentDialogueValue.name, labelStyle);
            EditorGUILayout.Space(10);
            
            labelStyle.fontSize = 12;
            EditorGUILayout.LabelField("Text", labelStyle);
            var textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fixedHeight = 60 };
            currentDialogueValue.dialogue = EditorGUILayout.TextArea(currentDialogueValue.dialogue, textAreaStyle, baseWidth);
            EditorGUILayout.Space(30);

            for(int i = 0; i < currentDialogueValue.nextDialogues.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.TextField("test");
                EditorGUILayout.EndVertical();
            }

            GUI.EndScrollView();
            EditorUtility.SetDirty(currentDialogueValue);
        }
    }
}

public class AddFactWindow : EditorWindow
{
    Facts facts;
    string tagString;
    FactValueType type;

    public static void Open()
    {
        AddFactWindow addFactWidow = EditorWindow.CreateInstance<AddFactWindow>();
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

public class AddDialogueWindow : EditorWindow
{
    string path => "Assets/DyConv/Container/Dialogues/";
    List<string> dialogueNames = new List<string>();
    string nameString;
    FactValueType type;

    public static void Open()
    {
        AddDialogueWindow addDialogueWidow = EditorWindow.CreateInstance<AddDialogueWindow>();
        string[] dialogueGuids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
        for (int i = 0; i < dialogueGuids.Length; i++) addDialogueWidow.dialogueNames.Add((AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(dialogueGuids[i])) as Dialogue).name);
        addDialogueWidow.ShowAsDropDown(new Rect(EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition), Vector2.zero), new Vector2(250, 20));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        nameString = EditorGUILayout.TextField(nameString);

        bool isDisable = false;

        if (string.IsNullOrWhiteSpace(nameString))
            isDisable = true;
        if (dialogueNames.Contains(nameString))
            isDisable = true;

        EditorGUI.BeginDisabledGroup(isDisable);
        if (GUILayout.Button("Add"))
        {
            Dialogue d = ScriptableObject.CreateInstance<Dialogue>();
            AssetDatabase.CreateAsset(d, path + nameString + ".asset");
            AssetDatabase.SaveAssets();
            this.Close();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
}
