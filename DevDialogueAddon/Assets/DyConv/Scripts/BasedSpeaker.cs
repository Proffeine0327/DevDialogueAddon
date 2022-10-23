using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasedSpeaker : MonoBehaviour
{
    [SerializeField] private string speakerTag;
    public string SpeakerTag => speakerTag;

    public abstract void Say();

    private void Awake() 
    {
        
    }
}
