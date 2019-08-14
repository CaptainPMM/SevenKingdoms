using System.Collections.Generic;

public class Spearmen : Soldier {
    public Spearmen() : base() { }
    public Spearmen(Soldier soldierToCopy) : base(soldierToCopy) { }

    protected override void SetupStats() {
        HP = 2;
        DP = 2;
        predictedStrength = 5;
        soldierName = "Spearmen";
        strengths = new Dictionary<SoldierType, float>() {
            { SoldierType.CAV_KNIGHTS, 2f }
        };
        goldCosts = 6;
    }
}