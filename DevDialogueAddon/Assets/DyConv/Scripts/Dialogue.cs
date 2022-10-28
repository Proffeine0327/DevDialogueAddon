using System;
using System.Linq;
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

    static List<Dialogue> GetAllDialogues()
    {
        List<Dialogue> dialogues = new List<Dialogue>();
        string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/DyConv/Container/Dialogues" });
        foreach (var guid in guids) dialogues.Add(AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guid)));

        return dialogues;
    }

    public static void UpdateDialogue()
    {
        var dialogueNames =
            from dName in Dialogue.allDialogues
            select dName.name;

        foreach (var dialogue in Dialogue.allDialogues)
        {
            for (int i = 0; i < dialogue.nextDialogueInfos.Count;)
            {
                if (!dialogueNames.Contains(dialogue.nextDialogueInfos[i].nextDialogueName))
                {
                    dialogue.nextDialogueInfos.RemoveAt(i);
                    EditorUtility.SetDirty(dialogue);
                }
                else
                {
                    for (int j = 0; j < dialogue.nextDialogueInfos[i].criteriaKeys.Count;)
                    {
                        if (!FactContainer.FactDictionary.ContainsKey(dialogue.nextDialogueInfos[i].criteriaKeys[j]))
                        {
                            dialogue.nextDialogueInfos[i].criteriaKeys.RemoveAt(j);
                            dialogue.nextDialogueInfos[i].compareTypes.RemoveAt(j);
                            dialogue.nextDialogueInfos[i].criteriaValues.RemoveAt(j);

                            EditorUtility.SetDirty(dialogue);
                        }
                        else
                            j++;
                    }

                    for (int j = 0; j < dialogue.nextDialogueInfos[i].modifyKeys.Count;)
                    {
                        if (!FactContainer.FactDictionary.ContainsKey(dialogue.nextDialogueInfos[i].modifyKeys[j]))
                        {
                            dialogue.nextDialogueInfos[i].modifyKeys.RemoveAt(j);
                            dialogue.nextDialogueInfos[i].modifyValues.RemoveAt(j);

                            EditorUtility.SetDirty(dialogue);
                        }
                        else
                            j++;
                    }
                    i++;
                }
            }
            AssetDatabase.SaveAssetIfDirty(dialogue);
        }
    }

    public string dialogueText;
    public int count;

    public List<NextDialogue> nextDialogueInfos = new List<NextDialogue>();
}
