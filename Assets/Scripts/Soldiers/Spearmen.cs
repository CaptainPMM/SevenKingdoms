using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spearmen : Soldier {
    public Spearmen() {
        HP = 1;
        DP = 2;
        predictedStrength = 2;
        soldierName = "Spearmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 2.5f },
            { SoldierType.CAV_KNIGHTS, 4f }
        };
        goldCosts = 6;
    }
}