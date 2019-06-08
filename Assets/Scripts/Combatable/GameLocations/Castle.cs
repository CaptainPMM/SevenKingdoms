﻿using UnityEngine;

public class Castle : GameLocation {
    // Start is called before the first frame update
    new void Start() {
        base.Start();

        BASE_GOLD_INCOME = 15;
        BASE_MANPOWER_INCOME = 10;

        buildableBuildings.AddRange(new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL,
            BuildingType.WOOD_MILL,
            BuildingType.BOW_MAKER,
            BuildingType.BLACKSMITH,
            BuildingType.STABLES
        });

        buildings.Add(new LocalAdministration());
        buildings.Add(new StoneWall());
        GetEffectsFromBuildings();
    }
}