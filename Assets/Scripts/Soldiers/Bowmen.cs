using System.Collections.Generic;

[System.Serializable]
public class Bowmen : Soldier {
    public Bowmen() {
        HP = 1;
        DP = 4;
        predictedStrength = 8;
        soldierName = "Bowmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 2f },
            { SoldierType.SPEARMEN, 2f }
        };
        goldCosts = 8;
    }
}