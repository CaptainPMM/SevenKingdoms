using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spearmen : Soldier {
    public Spearmen() {
        HP = 1;
        soldierName = "Spearmen";
        DP = 2;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 2.3f },
            { SoldierType.CAV, 3f },
            { SoldierType.CAV_KNIGHTS, 3f },
            { SoldierType.GIANTS, 1.5f },
            { SoldierType.DRAGONS, 1.25f }
        };
    }
}