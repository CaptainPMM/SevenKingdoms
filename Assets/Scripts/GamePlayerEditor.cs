using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GamePlayer))]
public class GamePlayerEditor : Editor {
    private int gold;
    private int manpower;
    private bool freeze;
    private GUIStyle headerStyle;

    public void OnEnable() {
        headerStyle = new GUIStyle();
        headerStyle.fontSize = 18;
        headerStyle.alignment = TextAnchor.UpperCenter;
        freeze = false;
    }

    public override void OnInspectorGUI() {
        GamePlayer player = (GamePlayer)target;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Player Editor", headerStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        gold = EditorGUILayout.IntField("Gold", gold);
        manpower = EditorGUILayout.IntField("Manpower", manpower);

        EditorGUILayout.Space();

        if (GUILayout.Button("ADD")) {
            EditorUtility.SetDirty(player);
            player.house.gold += gold;
            player.house.manpower += manpower;
        }

        if (GUILayout.Button("REDUCE")) {
            EditorUtility.SetDirty(player);
            player.house.gold -= gold;
            player.house.manpower -= manpower;
        }

        if (GUILayout.Button("SET")) {
            EditorUtility.SetDirty(player);
            player.house.gold = gold;
            player.house.manpower = manpower;
        }

        if (GUILayout.Button("Reset Input")) {
            gold = 0;
            manpower = 0;
        }

        if (GUILayout.Button("Revert Resources")) {
            EditorUtility.SetDirty(player);
            player.house.gold = 0;
            player.house.manpower = 0;
        }

        if (GUILayout.Button("Toggle Freeze")) {
            freeze = !freeze;
        }
        EditorGUILayout.LabelField("Freeze: " + freeze);

        if (freeze) {
            EditorUtility.SetDirty(player);
            player.house.gold = gold;
            player.house.manpower = manpower;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
    }
}