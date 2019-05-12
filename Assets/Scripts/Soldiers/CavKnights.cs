using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CavKnights : Soldier {
    public CavKnights() {
        HP = 4;
        soldierName = "Mounted Knights";
        DP = 2;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 3f },
            { SoldierType.BOWMEN, 4f }
        };
    }
}