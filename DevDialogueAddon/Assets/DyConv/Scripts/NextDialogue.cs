using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CompareType
{
    Equal, // ==
    Not,   // !=
    Less,  // <
    More,  // >
}

[Serializable]
public class NextDialogue
{
    public Dialogue nextDialogue;

    public List<string> criteriaKeys = new List<string>();
    public List<CompareType> compareTypes = new List<CompareType>();
    public List<int> criteriaValues = new List<int>();

    public List<string> modifyKeys = new List<string>();
    public List<int> modifyValues = new List<int>();
}
