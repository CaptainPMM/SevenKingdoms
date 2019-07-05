using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Soldiers {
    [SerializeField] private List<Soldier> conscripts = new List<Soldier>();
    [SerializeField] private List<Soldier> spearmen = new List<Soldier>();
    [SerializeField] private List<Soldier> swordsmen = new List<Soldier>();
    [SerializeField] private List<Soldier> bowmen = new List<Soldier>();
    [SerializeField] private List<Soldier> cavKnights = new List<Soldier>();

    public Soldiers() { }
    public Soldiers(Soldiers soldiersToCopy) {
        foreach (SoldierType st in CreateSoldierTypesArray()) {
            SetSoldierTypeNum(st, soldiersToCopy.GetSoldierTypeNum(st));
        }
    }

    private List<Soldier> FindSoldiersByType(SoldierType soldierType) {
        switch (soldierType) {
            case SoldierType.CONSCRIPTS:
                return conscripts;
            case SoldierType.SPEARMEN:
                return spearmen;
            case SoldierType.SWORDSMEN:
                return swordsmen;
            case SoldierType.BOWMEN:
                return bowmen;
            case SoldierType.CAV_KNIGHTS:
                return cavKnights;

            default:
                throw new Exception("Invalid SoldierType <" + soldierType + ">: could not be found");
        }
    }

    private static Soldier CreateSoldierInstance(SoldierType soldierType) {
        switch (soldierType) {
            case SoldierType.CONSCRIPTS:
                return new Conscripts();
            case SoldierType.SPEARMEN:
                return new Spearmen();
            case SoldierType.SWORDSMEN:
                return new Swordsmen();
            case SoldierType.BOWMEN:
                return new Bowmen();
            case SoldierType.CAV_KNIGHTS:
                return new CavKnights();

            default:
                throw new Exception("Invalid SoldierType <" + soldierType + ">: could not be found");
        }
    }

    public void SetSoldierType(SoldierType st, List<Soldier> soldiers) {
        FindSoldiersByType(st).Clear();
        FindSoldiersByType(st).AddRange(soldiers);
    }

    public int GetSoldierTypeNum(SoldierType soldierType) {
        return FindSoldiersByType(soldierType).Count; // Return number of men in one soldier type
    }

    public void SetSoldierTypeNum(SoldierType soldierType, int num) {
        FindSoldiersByType(soldierType).Clear();
        AddSoldierTypeNum(soldierType, num);
    }

    public void AddSoldierTypeNum(SoldierType soldierType, int num) {
        int counter = 0;
        while (counter < num) {
            FindSoldiersByType(soldierType).Add(CreateSoldierInstance(soldierType));
            counter++;
        }
    }

    public int GetNumSoldiersInTotal() {
        int numSoldiers = 0;
        foreach (SoldierType st in CreateSoldierTypesArray()) {
            numSoldiers += FindSoldiersByType(st).Count;
        }
        return numSoldiers;
    }

    public void AddSoldiers(Soldiers otherSoldiers) {
        foreach (SoldierType st in CreateSoldierTypesArray()) {
            FindSoldiersByType(st).AddRange(otherSoldiers.FindSoldiersByType(st));
        }
    }

    public void RemoveSoldiers(Soldiers otherSoldiers) {
        foreach (SoldierType st in CreateSoldierTypesArray()) {
            FindSoldiersByType(st).RemoveRange(0, Math.Min(FindSoldiersByType(st).Count, otherSoldiers.FindSoldiersByType(st).Count));
        }
    }

    public List<Soldier> ExtractSoldiers(SoldierType st, int amount) {
        List<Soldier> extractedSoldiers = new List<Soldier>();
        int soldiersToRemove = Math.Min(FindSoldiersByType(st).Count, amount);

        extractedSoldiers = FindSoldiersByType(st).GetRange(0, soldiersToRemove);
        FindSoldiersByType(st).RemoveRange(0, soldiersToRemove);
        return extractedSoldiers;
    }

    public int DealDamageToSoldierType(SoldierType soldierType, int amount) {
        int casualties = 0;
        List<Soldier> soldiers = FindSoldiersByType(soldierType);
        for (int i = 0; i < soldiers.Count; i++) {
            soldiers[i].HP -= amount;
            if (soldiers[i].HP <= 0) {
                // Soldier is dead
                amount = Math.Abs(soldiers[i].HP); // Reduce damage amount
                soldiers.RemoveAt(i); // Remove soldier
                casualties++;
            } else {
                // Amount is 0 now -> done
                return casualties;
            }
        }
        return casualties;
    }

    /**
        Duplicate for CreateSoldierInstance, but the name is better for use in other classes
     */
    public static Soldier GetSoldierTypeStats(SoldierType soldierType) {
        return CreateSoldierInstance(soldierType);
    }

    public static Array CreateSoldierTypesArray() {
        return Enum.GetValues(typeof(SoldierType));
    }
}