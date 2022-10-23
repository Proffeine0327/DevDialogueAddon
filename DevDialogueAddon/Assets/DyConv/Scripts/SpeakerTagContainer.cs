using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DyConv/SpeakerTagContainer", fileName = "SpeakerTagContainer", order = 1)]
public class SpeakerTagContainer : ScriptableObject
{
    public List<string> tags;
}
