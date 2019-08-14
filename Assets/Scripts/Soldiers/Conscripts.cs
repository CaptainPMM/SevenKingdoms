using System.Collections.Generic;

public class Conscripts : Soldier {
    public Conscripts() : base() { }
    public Conscripts(Soldier soldierToCopy) : base(soldierToCopy) { }

    protected override void SetupStats() {
        HP = 1;
        DP = 1;
        predictedStrength = 1;
        soldierName = "Conscripts";
        strengths = new Dictionary<SoldierType, float>();
        goldCosts = 2;
    }
}