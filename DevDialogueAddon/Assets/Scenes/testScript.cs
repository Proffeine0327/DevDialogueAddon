using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log(DyConvManager.SpeakerDictionary["Luna"].SpeakerTag);
    }
}
