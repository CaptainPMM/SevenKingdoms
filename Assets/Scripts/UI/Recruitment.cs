using UnityEngine;

public class Recruitment {
    private Soldiers soldiers;
    private Soldiers maxAvailableSoldiers;
    private GamePlayer player;
    private int currGoldCosts;
    private int currMpCosts;

    public Recruitment(GamePlayer player) {
        soldiers = new Soldiers();
        maxAvailableSoldiers = new Soldiers();
        this.player = player;
        CalculateMaxAvailableSoldiers();
    }

    private void CalculateMaxAvailableSoldiers() {
        // Calculate curr costs
        currGoldCosts = 0;
        currMpCosts = 0;

        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            int soldierNum = soldiers.GetSoldierTypeNum(st);
            currGoldCosts += soldierNum * Soldiers.GetSoldierTypeStats(st).goldCosts;
            currMpCosts += soldierNum;
        }

        // Calculate max available soldiers
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            int maxAvailableWithGold = Mathf.FloorToInt((float)(player.house.gold - currGoldCosts) / (float)Soldiers.GetSoldierTypeStats(st).goldCosts);
            int maxAvailableWithMp = player.house.manpower - currMpCosts;

            if (maxAvailableWithGold < maxAvailableWithMp) {
                maxAvailableSoldiers.SetSoldierTypeNum(st, maxAvailableWithGold);
            } else {
                maxAvailableSoldiers.SetSoldierTypeNum(st, maxAvailableWithMp);
            }
        }
    }

    public void Update() {
        CalculateMaxAvailableSoldiers();
    }

    public void SetRecruitSoldierTypeNum(SoldierType st, int amount) {
        soldiers.SetSoldierTypeNum(st, amount);
        CalculateMaxAvailableSoldiers();
    }

    public Soldiers GetRecruitSoldiers() {
        return soldiers;
    }

    public int GetMaxAvailableSoldierTypeNum(SoldierType st) {
        return maxAvailableSoldiers.GetSoldierTypeNum(st);
    }

    public int GetGoldCosts() {
        return currGoldCosts;
    }

    public int GetMpCosts() {
        return currMpCosts;
    }
}