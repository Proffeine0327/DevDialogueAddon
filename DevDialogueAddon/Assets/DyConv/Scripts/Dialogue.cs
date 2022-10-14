using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheckNModifyFactValueDictionary : SerializableDictionary<string, float> { }

[Serializable]
public class NextDialogue
{
    public Dialogue nextDialgue;
    public CheckNModifyFactValueDictionary criteria = new CheckNModifyFactValueDictionary();
    public CheckNModifyFactValueDictionary modification = new CheckNModifyFactValueDictionary();
}

[CreateAssetMenu(menuName = "DyConv/Dialogue", fileName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    public string dialogue;

    public List<NextDialogue> nextDialogues = new List<NextDialogue>();
}

