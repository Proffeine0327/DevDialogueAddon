using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyConvManager : MonoBehaviour
{
    private static DyConvManager manager;
    public static bool isManagerNull => manager == null;

    Dictionary<string, BasedSpeaker> speakerDictionary = new Dictionary<string, BasedSpeaker>();
    public static Dictionary<string, BasedSpeaker> SpeakerDictionary => manager.speakerDictionary;

    Dictionary<string, int> factDictionary = new Dictionary<string, int>();
    public static Dictionary<string, int> FactDictionary => manager.factDictionary;

    /// <summary>
    /// Spawn new DyConvManager and register it in DontDestoryOnLoad
    /// </summary>
    public static void Initialize()
    {
        if (manager == null)
        {
            GameObject dm = new GameObject("[DyConv]");
            manager = dm.AddComponent<DyConvManager>();
            DontDestroyOnLoad(dm);
        }

        foreach(var fact in FactContainer.FactDictionary) DyConvManager.FactDictionary.Add(fact.Key, fact.Value);
        foreach(var speakerName in SpeakerTagContainer.TagList) DyConvManager.SpeakerDictionary.Add(speakerName, null);
    }

    private void Awake()
    {

    }
}
