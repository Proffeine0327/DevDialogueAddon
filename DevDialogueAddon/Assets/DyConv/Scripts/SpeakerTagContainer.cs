using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "DyConv/SpeakerTagContainer", fileName = "SpeakerTagContainer", order = 1)]
public class SpeakerTagContainer : ScriptableObject, ISerializationCallbackReceiver
{
    public static SpeakerTagContainer container => AssetDatabase.LoadAssetAtPath<SpeakerTagContainer>("Assets/DyConv/Container/SpeakerTagContainer.asset") 
        ?? throw new NullReferenceException("SpeakerTagContainer or its parent file does not exist");

    private List<string> tagList = new List<string>();
    private HashSet<string> tags = new HashSet<string>();

    public static List<string> TagList => container.tagList;
    public static HashSet<string> Tags => container.tags;

    public void OnBeforeSerialize()
    {
        tagList.Clear();
        foreach(var ele in tags) tagList.Add(ele);
    }

    public void OnAfterDeserialize()
    {
        tags = new HashSet<string>();
        foreach(var ele in tagList) tags.Add(ele);
    }

    [ExecuteInEditMode, CustomEditor(typeof(SpeakerTagContainer))]
    public class SpeakerTagContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
}
