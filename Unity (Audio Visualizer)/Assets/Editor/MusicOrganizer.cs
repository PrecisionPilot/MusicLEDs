using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioVisualizer))]
public class MusicOrganizer : Editor {

    AudioVisualizer AudioVisualizer;
    private void OnEnable()
    {
        AudioVisualizer = (AudioVisualizer)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Sort"))
        {
            AudioVisualizer.Sort();
        }
        if (GUILayout.Button("Shuffle"))
        {
            AudioVisualizer.Shuffle();
        }
        EditorGUILayout.EndHorizontal();
    }
}
