using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Giants : Soldier {
    public Giants() {
        HP = 200;
        soldierName = "Giants";
        DP = 50;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 5f },
            { SoldierType.BOWMEN, 2f },
            { SoldierType.CAV, 1.5f },
            { SoldierType.CAV_KNIGHTS, 1.7f },
        };
    }
}