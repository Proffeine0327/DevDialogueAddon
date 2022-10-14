using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dialogueManager { get; set; }

    Dictionary<string, BaseSpeaker> speakerDictionary = new Dictionary<string, BaseSpeaker>();
    Dictionary<string, FactValue> factDictionary = new Dictionary<string, FactValue>();

    public static void CreateDialogueManager()
    {
        GameObject dm = new GameObject("[DyConv]");
        DontDestroyOnLoad(dm);

        dialogueManager = dm.AddComponent<DialogueManager>();
        Speakers speakers = AssetDatabase.LoadAssetAtPath<Speakers>("Assets/DyConv/Container/Speakers.asset");
        Facts facts = AssetDatabase.LoadAssetAtPath<Facts>("Assets/DyConv/Container/Facts.asset");
        foreach (var sp in speakers.speakerTags) dialogueManager.speakerDictionary.Add(sp, null);
        foreach (var fa in facts.allFacts) dialogueManager.factDictionary.Add(fa.Key, fa.Value);
    }

    public static void SetSpeaker(BaseSpeaker speaker)
    {
        dialogueManager.speakerDictionary[speaker.speakerName] = speaker;
    }

    public static BaseSpeaker GetSpeaker(string speakerTag)
    {
        return dialogueManager.speakerDictionary[speakerTag];
    }

    public static bool SetFactValue(string key, float value, FactValueType type)
    {
        if (dialogueManager.factDictionary.ContainsKey(key))
        {
            Debug.LogError("Key is not exist in Fact dictionary");
            return false;
        }

        float setValue = value;
        if (type == FactValueType.Int)
        {
            setValue = Convert.ToInt32(setValue);
        }
        else if(type == FactValueType.Float)
        {
            setValue = value;
        }
        else if (type == FactValueType.Bool)
        {
            if (Convert.ToBoolean(setValue) == true)
                setValue = 1;
            else
                setValue = 0;
        }
        else
        {
            Debug.LogError("Fact can only receive int, float, and bool types.");
            return false;
        }

        dialogueManager.factDictionary[key].value = setValue;
        return true;
    }

    public static T GetFactValue<T>(string key)
    {
        Type type = typeof(T);

        if (type == typeof(int))
        {
            object returnValue = Convert.ToInt32(dialogueManager.factDictionary[key].value);
            return (T)returnValue;
        }
        else if (type == typeof(float))
        {
            object returnValue = dialogueManager.factDictionary[key].value;
            return (T)returnValue;
        }
        else if (type == typeof(bool))
        {
            object returnValue = Convert.ToBoolean(dialogueManager.factDictionary[key].value);
            return (T)returnValue;
        }
        else
        {
            Debug.LogError("Fact can only receive int, float, and bool types.");
            return default(T);
        }
    }
}
