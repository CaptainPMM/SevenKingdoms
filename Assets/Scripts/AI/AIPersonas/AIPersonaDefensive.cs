using System.Collections.Generic;

public class AIPersonaDefensive : AIPersona {
    public AIPersonaDefensive() {
        name = "Defensive";
        _type = AIPersonaType.DEFENSIVE;

        _SoldierTypePriorities = new Dictionary<SoldierType, int>() {
            { SoldierType.CONSCRIPTS, 2 },
            { SoldierType.SPEARMEN, 2 },
            { SoldierType.SWORDSMEN, 3 },
            { SoldierType.BOWMEN, 2 },
            { SoldierType.CAV_KNIGHTS, 4 }
        };

        _BuildingTypePriorities = new Dictionary<BuildingType, int>() {
            { BuildingType.LOCAL_ADMINISTRATION, 2 }, // only in outposts because in castles they get built with highest prio already
            { BuildingType.MARKETPLACE, 5 },
            { BuildingType.OUTER_TOWN_RING, 7 },
            { BuildingType.WOODEN_WALL, 3 },
            { BuildingType.STONE_WALL, 6 },
            { BuildingType.ADVANCED_WALL, 14 },
            { BuildingType.WOOD_MILL, 6 },
            { BuildingType.BOW_MAKER, 4 },
            { BuildingType.BLACKSMITH, 6 },
            { BuildingType.STABLES, 4 },
            { BuildingType.BARRACKS, 1 },
            { BuildingType.DRILL_GROUND, 10 }
        };

        _MinMoveSoldiersNum = 20;

        _BaseLocationGarrisonMod = 0.8f;
        _BaseCastleGarrisonMod = 0.8f;

        _FleeingPoint = -5;
        _AttackingPoint = -8;

        _RecruitBuildGoldRatioToTransferToBuild = 0.75f;
        _RecruitBuildGoldRatioToTransferToRecruit = 0.2f;
        _GoldPoolSharedDistribution = 0.4f;

        _MinRecruitmentGold = 25;

        _LocalAdminBuildFrequency = 20;
    }
}