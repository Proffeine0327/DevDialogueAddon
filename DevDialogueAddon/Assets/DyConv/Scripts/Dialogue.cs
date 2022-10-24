using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DyConv/Dialogue", fileName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    public string dialogueText;
    public int count;

    public List<NextDialogue> nextDialogues = new List<NextDialogue>();
}
