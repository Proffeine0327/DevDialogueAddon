using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "DyConv/FactContainer", fileName = "FactContainer", order = 2)]
public class FactContainer : ScriptableObject, ISerializationCallbackReceiver
{
    public static FactContainer container => AssetDatabase.LoadAssetAtPath<FactContainer>("Assets/DyConv/Container/FactContainer.asset") ?? throw new NullReferenceException("FactContainer is not Exist");

    private List<string> keys = new List<string>();
    private List<FactValue> values = new List<FactValue>();
    private Dictionary<string, FactValue> factDictionary;
    
    public static Dictionary<string, FactValue> FactDictionary => container.factDictionary;
    public static List<string> Keys => container.keys;
    public static List<FactValue> Values => container.values;

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach(var kvp in factDictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        factDictionary = new Dictionary<string, FactValue>();

        if(keys.Count != values.Count)
            throw new Exception("Count of key list and value list is not match");
        
        for(int i = 0; i < keys.Count; i++) factDictionary.Add(keys[i], values[i]);
    }
}
