using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameLocation), true)]
[CanEditMultipleObjects]
public class GameLocationEditor : CombatableEditor {
    private int[] inputValues;
    private GameLocation inputGameLocation;
    private GUIStyle headerStyle;

    new public void OnEnable() {
        base.OnEnable();
        inputValues = new int[soldierTypes.Length];
        inputGameLocation = null;
        headerStyle = new GUIStyle();
        headerStyle.fontSize = 18;
        headerStyle.alignment = TextAnchor.UpperCenter;
    }

    public override void OnInspectorGUI() {
        GameLocation gl = (GameLocation)target;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Soldiers Editor", headerStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        int counter = 0;
        foreach (SoldierType st in soldierTypes) {
            inputValues[counter] = EditorGUILayout.IntField(st.ToString(), inputValues[counter]);
            counter++;
        }

        EditorGUILayout.Space();

        // Multi-object editing supported for this
        if (GUILayout.Button("Add Soldiers (multi)")) {
            foreach (GameLocation targetGL in targets) {
                EditorUtility.SetDirty(targetGL);
                targetGL.soldiers.AddSoldiers(CreateNewSoldiers());
            }
        }

        if (GUILayout.Button("Reset Input")) {
            inputValues = new int[soldierTypes.Length];
        }

        if (GUILayout.Button("Revert Soldiers")) {
            gl.soldiers = new Soldiers();
            SerializedProperty sp = new SerializedObject(gl).FindProperty("soldiers");
            sp.prefabOverride = false;
            sp.serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Add reachable location on both sides input
        EditorGUILayout.LabelField("Location Connector", headerStyle);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add reachable location on both:");
        inputGameLocation = (GameLocation)EditorGUILayout.ObjectField("Other GameLocation", inputGameLocation, typeof(GameLocation), true);
        if (inputGameLocation != null) {
            if (!inputGameLocation.Equals(gl)) {
                List<GameLocation> ownLocations = new List<GameLocation>(gl.reachableLocations);
                List<GameLocation> otherLocations = new List<GameLocation>(inputGameLocation.reachableLocations);

                if (!ownLocations.Contains(inputGameLocation)) {
                    EditorUtility.SetDirty(gl);
                    ownLocations.Add(inputGameLocation);
                    gl.reachableLocations = ownLocations.ToArray();
                }
                if (!otherLocations.Contains(gl)) {
                    EditorUtility.SetDirty(inputGameLocation);
                    otherLocations.Add(gl);
                    inputGameLocation.reachableLocations = otherLocations.ToArray();
                }
            }
            inputGameLocation = null; // Reset input for next location
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }

    private Soldiers CreateNewSoldiers() {
        Soldiers newSoldiers = new Soldiers();
        int i = 0;
        foreach (SoldierType st in soldierTypes) {
            newSoldiers.AddSoldierTypeNum(st, inputValues[i]);
            i++;
        }
        return newSoldiers;
    }
}