using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Combatable), true)]
public class CombatableEditor : Editor {
    protected System.Array soldierTypes;
    private GUIStyle titleStyle;

    public void OnEnable() {
        soldierTypes = Soldiers.CreateSoldierTypesArray();
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 14;
    }

    public override void OnInspectorGUI() {
        Combatable c = (Combatable)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Soldiers Summary", titleStyle);
        EditorGUILayout.Space();
        foreach (SoldierType st in soldierTypes) {
            EditorGUILayout.IntField("    " + st.ToString(), c.soldiers.GetSoldierTypeNum(st));
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}