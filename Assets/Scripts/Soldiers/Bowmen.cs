using System.Collections.Generic;

public class Bowmen : Soldier {
    public Bowmen() : base() { }
    public Bowmen(Soldier soldierToCopy) : base(soldierToCopy) { }

    protected override void SetupStats() {
        HP = 1;
        DP = 4;
        predictedStrength = 8;
        soldierName = "Bowmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CONSCRIPTS, 2f },
            { SoldierType.SPEARMEN, 2f }
        };
        goldCosts = 8;
    }
}