using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasedSpeaker : MonoBehaviour
{
    [SerializeField] private string speakerTag;
    
    public string SpeakerTag 
    {
        get
        {
            if(SpeakerTagContainer.Tags.Contains(speakerTag))
                return speakerTag;
            else
            {
                speakerTag = null;
                return null;
            }
        }
    }

    public abstract void Say(string dialogueText);

    private void Awake() 
    {
        if(DyConvManager.isManagerNull)
            DyConvManager.Initialize();
        
        DyConvManager.SpeakerDictionary[speakerTag] = this;
    }
}
