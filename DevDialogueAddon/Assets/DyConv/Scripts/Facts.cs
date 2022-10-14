using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum FactValueType
{
    Int,
    Float,
    Bool
}

[Serializable]
public class FactValue
{
    public float value;
    public FactValueType type;
}

[Serializable]
public class FactDictionary : SerializableDictionary<string, FactValue> { }

[CreateAssetMenu(menuName = "DyConv/Facts", fileName = "Facts", order = 0)]
public class Facts : ScriptableObject
{
    public FactDictionary allFacts = new FactDictionary();

    public bool SetFactValue(string key, float value)
    {
        if(!allFacts.keys.Contains(key))
        {
            Debug.LogError($"Fact dictioanry does not contain \"{key}\"");
            return false;
        }
        allFacts[key].value = value;
        return true;
    }
 
    public bool SetFactType(string key, FactValueType type)
    {
        if(!allFacts.keys.Contains(key))
        {
            Debug.LogError($"Fact dictioanry does not contain \"{key}\"");
            return false;
        }

        allFacts[key].type = type;
        return true;
    }

    public FactValue GetFact(string key) => allFacts[key];
}

[ExecuteInEditMode, CustomPropertyDrawer(typeof(FactDictionary))]
public class FactDictionaryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect fixedPosition = new Rect(position);

        SerializedProperty _keys = property.FindPropertyRelative("keys");
        SerializedProperty _values = property.FindPropertyRelative("values");

        fixedPosition.width /= 2;

        EditorGUI.BeginDisabledGroup(true);
        for (int i = 0; i < _keys.arraySize; i++)
        {
            EditorGUI.TextField(fixedPosition, _keys.GetArrayElementAtIndex(i).stringValue);
            fixedPosition.x += fixedPosition.width;

            SerializedProperty _factValue = _values.GetArrayElementAtIndex(i);
            string showValue = "";
            if (_factValue.FindPropertyRelative("type").enumValueIndex == (int)FactValueType.Bool)
                showValue = Convert.ToBoolean(_factValue.FindPropertyRelative("value").floatValue).ToString();
            else
                showValue = _factValue.FindPropertyRelative("value").floatValue.ToString();

            EditorGUI.TextField(fixedPosition, showValue);
            fixedPosition.x -= fixedPosition.width;
            fixedPosition.y += 20;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndProperty();
    }
}
