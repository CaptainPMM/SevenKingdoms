using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPersona {
    public string name;
    public AIPersonaType type {
        get {
            return _type;
        }
    }
    protected AIPersonaType _type;

    public Dictionary<SoldierType, int> SoldierTypePriorities {
        get {
            return _SoldierTypePriorities;
        }
    }
    protected Dictionary<SoldierType, int> _SoldierTypePriorities;

    public Dictionary<BuildingType, int> BuildingTypePriorities {
        get {
            return _BuildingTypePriorities;
        }
    }
    protected Dictionary<BuildingType, int> _BuildingTypePriorities;

    public int MinMoveSoldiersNum {
        get {
            return _MinMoveSoldiersNum;
        }
    }
    protected int _MinMoveSoldiersNum;

    // Amount of soldiers to stay in any location as base.
    // Value between 1 and 0.
    // 1 means nobody stays; 0 means 100.
    // 0.5 means 50 soldiers stay.
    public float BaseLocationGarrisonMod {
        get {
            return _BaseLocationGarrisonMod;
        }
    }
    protected float _BaseLocationGarrisonMod;

    public float BaseCastleGarrisonMod {
        get {
            return _BaseCastleGarrisonMod;
        }
    }
    protected float _BaseCastleGarrisonMod;

    // Used for CompareStrength(enemy, ally) < <this_value>
    // 0 means flee if enemy is stronger.
    // 10 means flee already although ally troops may be stronger.
    // -10 means flee later, also fight if ally troops weaker.
    public float FleeingPoint {
        get {
            return _FleeingPoint;
        }
    }
    protected float _FleeingPoint;

    // Used for CompareStrength(ally, enemy) < <this_value>
    // 0 means attack if allies are stronger.
    // -> They attack if slightly stronger (dangerous).
    // 10 means attack already although enemy troops may be stronger.
    // -10 means attack later, if definetly stronger than enemy.
    public float AttackingPoint {
        get {
            return _AttackingPoint;
        }
    }
    protected float _AttackingPoint;

    // Transfer gold only to build pool if recruitGold / buildGold > <this>
    // > 1 if recruit pool can get higher than build pool.
    // < 1 if build pool should be higher all the time.
    public float RecruitBuildGoldRatioToTransferToBuild {
        get {
            return _RecruitBuildGoldRatioToTransferToBuild;
        }
    }
    protected float _RecruitBuildGoldRatioToTransferToBuild;

    // Transfer gold only to recruit pool if recruitGold / buildGold < <this>
    // Value between 0 and 1 at which percentage gold only gets transfered to recruit pool.
    public float RecruitBuildGoldRatioToTransferToRecruit {
        get {
            return _RecruitBuildGoldRatioToTransferToRecruit;
        }
    }
    protected float _RecruitBuildGoldRatioToTransferToRecruit;

    // How to split the gold on shared distribution.
    // Recruitment pool gets the gold * <this>.
    // Build the rest.
    public float GoldPoolSharedDistribution {
        get {
            return _GoldPoolSharedDistribution;
        }
    }
    protected float _GoldPoolSharedDistribution;

    public int MinRecruitmentGold {
        get {
            return _MinRecruitmentGold;
        }
    }
    protected int _MinRecruitmentGold;

    // The AI handle steps needed before AI definetly builds a local admin
    public int LocalAdminBuildFrequency {
        get {
            return _LocalAdminBuildFrequency;
        }
    }
    protected int _LocalAdminBuildFrequency;

    public static AIPersona GetRandomAIPersona() {
        int rand = Random.Range(0, System.Enum.GetValues(typeof(AIPersonaType)).Length);
        return GetAIPersona((AIPersonaType)rand);
    }

    public static AIPersona GetAIPersona(AIPersonaType personaType) {
        switch (personaType) {
            case AIPersonaType.BALANCED:
                return new AIPersonaBalanced();
            case AIPersonaType.AGGRESSIVE:
                return new AIPersonaAggressive();
            case AIPersonaType.DEFENSIVE:
                return new AIPersonaDefensive();

            default:
                throw new System.Exception("Invalid PersonaType <" + personaType + ">: could not be found");
        }
    }
}