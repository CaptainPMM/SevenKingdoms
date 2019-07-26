using System.Collections.Generic;

public class AIPersonaRambo : AIPersona {
    public AIPersonaRambo() {
        name = "Rambo";
        _type = AIPersonaType.RAMBO;

        _SoldierTypePriorities = new Dictionary<SoldierType, int>() {
            { SoldierType.CONSCRIPTS, 4 },
            { SoldierType.SPEARMEN, 3 },
            { SoldierType.SWORDSMEN, 2 },
            { SoldierType.BOWMEN, 2 },
            { SoldierType.CAV_KNIGHTS, 5 }
        };

        _BuildingTypePriorities = new Dictionary<BuildingType, int>() {
            { BuildingType.LOCAL_ADMINISTRATION, 1 }, // only in outposts because in castles they get built with highest prio already
            { BuildingType.MARKETPLACE, 3 },
            { BuildingType.OUTER_TOWN_RING, 8 },
            { BuildingType.WOODEN_WALL, 1 },
            { BuildingType.STONE_WALL, 3 },
            { BuildingType.ADVANCED_WALL, 8 },
            { BuildingType.WOOD_MILL, 8 },
            { BuildingType.BOW_MAKER, 3 },
            { BuildingType.BLACKSMITH, 3 },
            { BuildingType.STABLES, 8 },
            { BuildingType.BARRACKS, 4 },
            { BuildingType.DRILL_GROUND, 14 }
        };

        _MinMoveSoldiersNum = 15;

        _BaseLocationGarrisonMod = 1f;
        _BaseCastleGarrisonMod = 1f;

        _FleeingPoint = -50;
        _AttackingPoint = 5;

        _RecruitBuildGoldRatioToTransferToBuild = 0.75f;
        _RecruitBuildGoldRatioToTransferToRecruit = 0.2f;
        _GoldPoolSharedDistribution = 0.75f;

        _MinRecruitmentGold = 10;

        _LocalAdminBuildFrequency = 30;
    }
}