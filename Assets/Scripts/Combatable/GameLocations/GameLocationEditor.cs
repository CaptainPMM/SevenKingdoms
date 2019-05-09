using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameLocation), true)]
public class GameLocationEditor : CombatableEditor {
    private int[] inputValues;
    private GUIStyle headerStyle;

    new public void OnEnable() {
        base.OnEnable();
        inputValues = new int[soldierTypes.Length];
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

        if (GUILayout.Button("Add Soldiers")) {
            EditorUtility.SetDirty(gl);

            Soldiers newSoldiers = new Soldiers();
            int i = 0;
            foreach (SoldierType st in soldierTypes) {
                newSoldiers.AddSoldierTypeNum(st, inputValues[i]);
                i++;
            }
            gl.soldiers.AddSoldiers(newSoldiers);
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
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}