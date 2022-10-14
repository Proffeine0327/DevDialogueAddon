using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class BaseSpeaker : MonoBehaviour
{
    public string speakerName;

    private void Awake()
    {
        if (DialogueManager.dialogueManager == null)
            DialogueManager.CreateDialogueManager();

        if (DialogueManager.GetSpeaker(speakerName) != null)
        {
            Debug.LogWarning("Scene cannot place more than one speaker with the same tag. '" + name + "' is disabled");
            gameObject.SetActive(false);
        }
        else
            DialogueManager.SetSpeaker(this);
    }

    public virtual void Say() { }
}

[ExecuteInEditMode, CustomEditor(typeof(Speaker))]
public class SpeakerEditor : Editor
{
    Speaker speaker;
    SerializedProperty _speakerName;
    Speakers speakers;

    private void OnEnable()
    {
        speaker = target as Speaker;
        _speakerName = serializedObject.FindProperty("speakerName");
        speakers = AssetDatabase.LoadAssetAtPath<Speakers>("Assets/DyConv/Container/Speakers.asset");

        if (speakers == null)
        {
            speakers = ScriptableObject.CreateInstance<Speakers>();
            AssetDatabase.CreateAsset(speakers, "Assets/DyConv/Container/Speakers.asset");
            EditorUtility.SetDirty(speakers);
            AssetDatabase.SaveAssetIfDirty(speakers);
        }
    }

    public override void OnInspectorGUI()
    {
        if (!speakers.speakerTags.Contains(_speakerName.stringValue))
        {
            _speakerName.stringValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.BeginHorizontal();
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label("Speaker", GUILayout.MaxWidth(100));
        
        if (GUILayout.Button(_speakerName.stringValue, EditorStyles.popup))
        {
            SearchPopupWindow.Open(SearchType.Speaker, (x) =>
            {
                _speakerName.stringValue = x;
                serializedObject.ApplyModifiedProperties();
            });
        }
        EditorGUILayout.EndHorizontal();
    }
}


