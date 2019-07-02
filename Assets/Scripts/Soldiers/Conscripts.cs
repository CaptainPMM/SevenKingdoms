using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Conscripts : Soldier {
    public Conscripts() {
        HP = 1;
        DP = 1;
        predictedStrength = 1;
        soldierName = "Conscripts";
        strengths = new Dictionary<SoldierType, float>();
        goldCosts = 2;
    }
}