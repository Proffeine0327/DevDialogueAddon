using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddNewDialogueWindow : EditorWindow
{
    string dialogueName;
    List<string> dialogueNameList = new List<string>();

    public static void Open()
    {
        var window = EditorWindow.CreateInstance<AddNewDialogueWindow>();

        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(250, 20));
    }

    private void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
        dialogueNameList.Clear();
        foreach (var guid in guids) dialogueNameList.Add(AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guid)).name);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        dialogueName = EditorGUILayout.TextField(dialogueName);

        bool isDisable = false;
        if (string.IsNullOrEmpty(dialogueName))
            isDisable = true;
        if (dialogueNameList.Contains(dialogueName))
            isDisable = true;

        EditorGUI.BeginDisabledGroup(isDisable);
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            var dialogue = ScriptableObject.CreateInstance<Dialogue>();
            AssetDatabase.CreateAsset(dialogue, $"Assets/DyConv/Container/Dialogues/{dialogueName}.asset");
            AssetDatabase.SaveAssets();
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
