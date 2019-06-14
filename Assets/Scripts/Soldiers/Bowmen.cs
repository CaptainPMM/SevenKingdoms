using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Bowmen : Soldier {
    public Bowmen() {
        HP = 1;
        soldierName = "Bowmen";
        DP = 1;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 2f },
            { SoldierType.SPEARMEN, 2f }
        };
        goldCosts = 8;
    }
}