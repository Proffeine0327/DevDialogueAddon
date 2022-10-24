using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DyConv/Dialogue", fileName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [SerializeField] private string dialogueText;
    public string DialogueText => dialogueText;

    public List<NextDialogue> nextDialogues = new List<NextDialogue>();
}
