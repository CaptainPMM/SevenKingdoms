using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Conscripts : Soldier {
    public Conscripts() {
        HP = 1;
        soldierName = "Conscripts";
        DP = 1;
        strengths = new Dictionary<SoldierType, float>();
    }
}