using System.Collections.Generic;

[System.Serializable]
public class Swordsmen : Soldier {
    public Swordsmen() {
        HP = 4;
        DP = 2;
        predictedStrength = 10;
        soldierName = "Swordsmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 1.5f },
            { SoldierType.SPEARMEN, 2f },
            { SoldierType.BOWMEN, 2f }
        };
        goldCosts = 12;
    }
}