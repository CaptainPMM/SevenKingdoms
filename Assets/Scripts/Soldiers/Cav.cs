using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cav : Soldier {
    public Cav() {
        HP = 1;
        soldierName = "Cavalry";
        DP = 2;
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 5f },
            { SoldierType.SWORDSMEN, 2f },
            { SoldierType.BOWMEN, 2.5f }
        };
    }
}