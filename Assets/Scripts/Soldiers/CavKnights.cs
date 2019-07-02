using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CavKnights : Soldier {
    public CavKnights() {
        HP = 4;
        DP = 2;
        predictedStrength = 5;
        soldierName = "Mounted Knights";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 3f },
            { SoldierType.BOWMEN, 4f }
        };
        goldCosts = 25;
    }
}