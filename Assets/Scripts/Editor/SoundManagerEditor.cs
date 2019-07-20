using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor {
    private GUIStyle headerStyle;
    private string statusTxt;
    private int counter;

    public void OnEnable() {
        headerStyle = new GUIStyle();
        headerStyle.fontSize = 18;
        headerStyle.alignment = TextAnchor.UpperCenter;
        statusTxt = "";
        counter = 0;
    }

    public override void OnInspectorGUI() {
        SoundManager sm = (SoundManager)target;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Sound Loader", headerStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Load all new sound files from:");
        EditorGUILayout.LabelField("/Assets/Resources/Sounds/2D/<folders_into_category>");
        if (GUILayout.Button("LOAD NEW FILES")) {
            Debug.Log("### SOUND LOADER ###");
            Debug.Log("Init...");
            LoadNewSoundFiles(sm);
            Debug.Log("## DONE ##");
        }
        EditorGUILayout.LabelField(statusTxt);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }

    private void LoadNewSoundFiles(SoundManager sm) {
        EditorUtility.SetDirty(sm);
        counter = 0;

        Debug.Log("> Loading all sound files [0/3] ...");
        AddNewSoundsFrom("Sounds/2D/UI", ref sm.uiSounds);

        Debug.Log("> Loading all sound files [1/3] ...");
        AddNewSoundsFrom("Sounds/2D/Music", ref sm.musicSounds);

        Debug.Log("> Loading all sound files [2/3] ...");
        AddNewSoundsFrom("Sounds/2D/Effects", ref sm.effectSounds);

        statusTxt = "=> " + counter + " new files added";
        Debug.Log("> In total " + counter + " new files added");
    }

    private void AddNewSoundsFrom(string path, ref Sound[] existingSounds) {
        // Load all audio clips from the path
        Debug.Log(">> Path: " + path);
        AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
        Debug.Log(">> Loaded " + clips.Length + " files");

        // Ignore duplicates
        List<AudioClip> newClips = new List<AudioClip>();
        foreach (AudioClip clip in clips) {
            if (System.Array.Find(existingSounds, s => s.name == clip.name) == null) {
                newClips.Add(clip);
                counter++;
                Debug.Log(">>> New file found: " + clip.name);
            }
        }

        // Add new clips
        Debug.Log(">> Adding <" + newClips.Count + "> new files to existing array...");
        List<Sound> newSoundsList = new List<Sound>();
        newSoundsList.AddRange(existingSounds); // Add old sounds
        foreach (AudioClip c in newClips) newSoundsList.Add(new Sound(c.name, c, false, 0.5f)); // Add new sounds
        existingSounds = newSoundsList.ToArray();
    }
}