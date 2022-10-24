using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode, CustomEditor(typeof(Speaker))]
public class SpeakerEditor : Editor
{
    SerializedProperty _speakerTag;

    private void OnEnable() 
    {
        _speakerTag = serializedObject.FindProperty("speakerTag");   
    }

    public override void OnInspectorGUI()
    {
        if(!SpeakerTagContainer.Tags.Contains(_speakerTag.stringValue))
        {
            _speakerTag.stringValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Speaker Tag");
        if(GUILayout.Button(_speakerTag.stringValue, EditorStyles.popup))
        {
            SpeakerSearchablePopupWindow.Open((x) => {
                 _speakerTag.stringValue = x;
                serializedObject.ApplyModifiedProperties();
            });
        }
        EditorGUILayout.EndHorizontal();
    }
}
