using System.Collections.Generic;

public class AIPersonaAggressive : AIPersona {
    public AIPersonaAggressive() {
        name = "Aggressive";
        _type = AIPersonaType.AGGRESSIVE;

        _SoldierTypePriorities = new Dictionary<SoldierType, int>() {
            { SoldierType.CONSCRIPTS, 3 },
            { SoldierType.SPEARMEN, 3 },
            { SoldierType.SWORDSMEN, 2 },
            { SoldierType.BOWMEN, 2 },
            { SoldierType.CAV_KNIGHTS, 4 }
        };

        _BuildingTypePriorities = new Dictionary<BuildingType, int>() {
            { BuildingType.LOCAL_ADMINISTRATION, 1 }, // only in outposts because in castles they get built with highest prio already
            { BuildingType.MARKETPLACE, 5 },
            { BuildingType.OUTER_TOWN_RING, 7 },
            { BuildingType.WOODEN_WALL, 1 },
            { BuildingType.STONE_WALL, 4 },
            { BuildingType.ADVANCED_WALL, 10 },
            { BuildingType.WOOD_MILL, 6 },
            { BuildingType.BOW_MAKER, 3 },
            { BuildingType.BLACKSMITH, 4 },
            { BuildingType.STABLES, 4 },
            { BuildingType.BARRACKS, 3 },
            { BuildingType.DRILL_GROUND, 12 }
        };

        _MinMoveSoldiersNum = 5;

        _BaseLocationGarrisonMod = 1f;
        _BaseCastleGarrisonMod = 0.8f;

        _FleeingPoint = -4;
        _AttackingPoint = 0;

        _RecruitBuildGoldRatioToTransferToBuild = 0.75f;
        _RecruitBuildGoldRatioToTransferToRecruit = 0.2f;
        _GoldPoolSharedDistribution = 0.66f;

        _MinRecruitmentGold = 10;

        _LocalAdminBuildFrequency = 35;
    }
}