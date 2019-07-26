using System.Collections.Generic;

public class AIPersonaWealthy : AIPersona {
    public AIPersonaWealthy() {
        name = "Wealthy";
        _type = AIPersonaType.WEALTHY;

        _SoldierTypePriorities = new Dictionary<SoldierType, int>() {
            { SoldierType.CONSCRIPTS, 2 },
            { SoldierType.SPEARMEN, 3 },
            { SoldierType.SWORDSMEN, 5 },
            { SoldierType.BOWMEN, 5 },
            { SoldierType.CAV_KNIGHTS, 8 }
        };

        _BuildingTypePriorities = new Dictionary<BuildingType, int>() {
            { BuildingType.LOCAL_ADMINISTRATION, 3 }, // only in outposts because in castles they get built with highest prio already
            { BuildingType.MARKETPLACE, 7 },
            { BuildingType.OUTER_TOWN_RING, 5 },
            { BuildingType.WOODEN_WALL, 3 },
            { BuildingType.STONE_WALL, 6 },
            { BuildingType.ADVANCED_WALL, 14 },
            { BuildingType.WOOD_MILL, 6 },
            { BuildingType.BOW_MAKER, 5 },
            { BuildingType.BLACKSMITH, 6 },
            { BuildingType.STABLES, 8 },
            { BuildingType.BARRACKS, 2 },
            { BuildingType.DRILL_GROUND, 10 }
        };

        _MinMoveSoldiersNum = 30;

        _BaseLocationGarrisonMod = 0.8f;
        _BaseCastleGarrisonMod = 0.9f;

        _FleeingPoint = -25;
        _AttackingPoint = -10;

        _RecruitBuildGoldRatioToTransferToBuild = 0.75f;
        _RecruitBuildGoldRatioToTransferToRecruit = 0.2f;
        _GoldPoolSharedDistribution = 0.4f;

        _MinRecruitmentGold = 25;

        _LocalAdminBuildFrequency = 15;
    }
}