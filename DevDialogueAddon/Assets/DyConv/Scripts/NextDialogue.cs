using System;
using System.Linq;
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
    [SerializeField] private string nextDialogueName;

    public string NextDialogueName 
    {
        get 
        {
            var allDialogueNames =
                from dname in Dialogue.allDialogues
                select dname.name;
            
            if(allDialogueNames.Contains(nextDialogueName))
            {
                return nextDialogueName;
            }
            else
            {
                nextDialogueName = null;
                return null;
            }
        }

        set
        {
            nextDialogueName = value;
        }
    }

    public List<string> criteriaKeys = new List<string>();
    public List<CompareType> compareTypes = new List<CompareType>();
    public List<int> criteriaValues = new List<int>();

    public List<string> modifyKeys = new List<string>();
    public List<int> modifyValues = new List<int>();
}
