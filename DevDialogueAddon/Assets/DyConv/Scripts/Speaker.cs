using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : BasedSpeaker
{
    //Plese do not use Awake, Start, Update and etc...
    public override void Say(string dialogueText)
    {
        Debug.Log(dialogueText);
    }
}
