using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CompareType
{
    [Description("==")]
    Equal,
    [Description("<=")]
    Less,
    [Description(">=")]
    More,
    [Description("!=")]
    Not
}

[Serializable]
public class NextDialogue
{
    public Dialogue nextDialogue;

    public List<string> criteriaKeys = new List<string>();
    public List<CompareType> compareTypes = new List<CompareType>();
    public List<float> criteriaValues = new List<float>();

    public List<string> modifyKeys = new List<string>();
    public List<float> modifyValues = new List<float>();
}
