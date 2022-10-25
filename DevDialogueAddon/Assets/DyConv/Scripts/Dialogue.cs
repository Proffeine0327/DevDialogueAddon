using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "DyConv/Dialogue", fileName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    public static List<Dialogue> allDialogues => GetAllDialogues();
    public static Dialogue GetDialogue(string dialogueName) => AssetDatabase.LoadAssetAtPath<Dialogue>($"Assets/DyConv/Container/Dialogues/{dialogueName}.asset")
        ?? throw new System.Exception($"Dialogue, \"{dialogueName}\" is not exist");

    public static List<Dialogue> GetAllDialogues()
    {
        List<Dialogue> dialogues = new List<Dialogue>();
        string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
        foreach (var guid in guids) dialogues.Add(AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guid)));

        return dialogues;
    }


    public string dialogueText;
    public int count;

    public List<NextDialogue> nextDialogues = new List<NextDialogue>();
}
