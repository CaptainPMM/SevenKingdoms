using System.Collections.Generic;

[System.Serializable]
public class Spearmen : Soldier {
    public Spearmen() {
        HP = 2;
        DP = 2;
        predictedStrength = 4;
        soldierName = "Spearmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CAV_KNIGHTS, 2f }
        };
        goldCosts = 6;
    }
}