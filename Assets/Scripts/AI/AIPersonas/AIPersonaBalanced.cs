using System.Collections.Generic;

public class AIPersonaBalanced : AIPersona {
    public AIPersonaBalanced() {
        name = "Balanced";
        _type = AIPersonaType.BALANCED;

        _SoldierTypePriorities = new Dictionary<SoldierType, int>() {
            { SoldierType.CONSCRIPTS, 2 },
            { SoldierType.SPEARMEN, 2 },
            { SoldierType.SWORDSMEN, 3 },
            { SoldierType.BOWMEN, 2 },
            { SoldierType.CAV_KNIGHTS, 4 }
        };

        _BuildingTypePriorities = new Dictionary<BuildingType, int>() {
            { BuildingType.LOCAL_ADMINISTRATION, 1 }, // only in outposts because in castles they get built with highest prio already
            { BuildingType.MARKETPLACE, 5 },
            { BuildingType.OUTER_TOWN_RING, 7 },
            { BuildingType.WOODEN_WALL, 1 },
            { BuildingType.STONE_WALL, 4 },
            { BuildingType.ADVANCED_WALL, 12 },
            { BuildingType.WOOD_MILL, 6 },
            { BuildingType.BOW_MAKER, 4 },
            { BuildingType.BLACKSMITH, 6 },
            { BuildingType.STABLES, 4 },
            { BuildingType.BARRACKS, 1 },
            { BuildingType.DRILL_GROUND, 10 }
        };

        _MinMoveSoldiersNum = 10;

        _BaseLocationGarrisonMod = 0.9f;
        _BaseCastleGarrisonMod = 0.8f;

        _FleeingPoint = -2;
        _AttackingPoint = -5;

        _RecruitBuildGoldRatioToTransferToBuild = 0.75f;
        _RecruitBuildGoldRatioToTransferToRecruit = 0.2f;
        _GoldPoolSharedDistribution = 0.5f;

        _MinRecruitmentGold = 25;

        _LocalAdminBuildFrequency = 25;
    }
}