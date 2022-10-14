using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Speakers : ScriptableObject
{
    public List<string> speakerTags = new List<string>();
}

[ExecuteInEditMode, CustomEditor(typeof(Speakers))]
public class SpeakersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.EndDisabledGroup();
    }
}


