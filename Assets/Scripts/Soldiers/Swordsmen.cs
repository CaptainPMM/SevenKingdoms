using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Swordsmen : Soldier {
    public Swordsmen() {
        HP = 2;
        DP = 1;
        predictedStrength = 3;
        soldierName = "Swordsmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SPEARMEN, 2f },
            { SoldierType.BOWMEN, 3f }
        };
        goldCosts = 12;
    }
}