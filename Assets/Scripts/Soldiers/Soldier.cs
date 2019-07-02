using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Soldier {
    public int HP;
    [HideInInspector] public int DP;
    [HideInInspector] public int predictedStrength;
    [HideInInspector] public string soldierName;
    [HideInInspector] public Dictionary<SoldierType, float> strengths;
    [HideInInspector] public int goldCosts;
}