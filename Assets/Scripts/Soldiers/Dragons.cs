using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dragons : Soldier {
    public Dragons() {
        HP = 500;
        soldierName = "Dragons";
        DP = 1000;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SPEARMEN, 5f },
            { SoldierType.SWORDSMEN, 4f },
            { SoldierType.BOWMEN, 5f },
            { SoldierType.CAV, 3f },
            { SoldierType.CAV_KNIGHTS, 4f },
            { SoldierType.GIANTS, 3f }
        };
    }
}