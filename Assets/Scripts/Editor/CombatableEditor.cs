using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Combatable), true)]
public class CombatableEditor : Editor {
    protected System.Array soldierTypes;
    private GUIStyle titleStyle;
    private GUIStyle textAreaStyle;
    private bool showHP;

    public void OnEnable() {
        soldierTypes = Soldiers.CreateSoldierTypesArray();

        titleStyle = new GUIStyle();
        titleStyle.fontSize = 14;

        textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;

        showHP = false;
    }

    public override void OnInspectorGUI() {
        Combatable c = (Combatable)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Soldiers Summary", titleStyle);
        EditorGUILayout.Space();
        foreach (SoldierType st in soldierTypes) {
            EditorGUILayout.LabelField(st.ToString(), c.soldiers.GetSoldierTypeNum(st).ToString());
        }
        if (GUILayout.Button("Toggle HP")) {
            showHP = !showHP;
        }
        if (showHP) {
            foreach (SoldierType st in soldierTypes) {
                EditorGUILayout.LabelField(st.ToString());
                string hpData = "";
                foreach (Soldier s in c.soldiers.FindSoldiersByType(st)) {
                    hpData += s.HP + ",";
                }
                EditorGUILayout.TextArea(hpData, textAreaStyle);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}