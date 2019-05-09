using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CavKnights : Soldier {
    public CavKnights() {
        HP = 4;
        soldierName = "Knights";
        DP = 2;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 2.2f },
            { SoldierType.BOWMEN, 2.6f },
            { SoldierType.CAV, 2f }
        };
    }
}