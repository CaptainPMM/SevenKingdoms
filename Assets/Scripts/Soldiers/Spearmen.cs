using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spearmen : Soldier {
    public Spearmen() {
        HP = 1;
        soldierName = "Spearmen";
        DP = 2;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 2.5f },
            { SoldierType.CAV_KNIGHTS, 4f }
        };
    }
}