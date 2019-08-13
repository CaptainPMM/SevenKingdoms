using System.Collections.Generic;

public abstract class Soldier {
    public byte HP;
    public byte DP;
    public byte predictedStrength;
    public string soldierName;
    public Dictionary<SoldierType, float> strengths;
    public byte goldCosts;

    public Soldier() {
        SetupStats();
    }
    public Soldier(Soldier soldierToCopy) {
        SetupStats();
        if (soldierToCopy != null) {
            HP = soldierToCopy.HP;
        }
    }

    protected abstract void SetupStats();
}