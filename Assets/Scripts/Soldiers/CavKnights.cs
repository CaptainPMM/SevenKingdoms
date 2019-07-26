using System.Collections.Generic;

[System.Serializable]
public class CavKnights : Soldier {
    public CavKnights() {
        HP = 5;
        DP = 5;
        predictedStrength = 15;
        soldierName = "Mounted Knights";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 1.5f },
            { SoldierType.SWORDSMEN, 1.5f },
            { SoldierType.BOWMEN, 2f }
        };
        goldCosts = 20;
    }
}