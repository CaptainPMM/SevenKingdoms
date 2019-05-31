﻿using UnityEngine;

public class Neutral : House {
    public Neutral() {
        _houseName = "Neutral";
        _color = new Color32(146, 146, 146, 255);
        _houseFlag = null;
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.WOODEN_WALL,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL
        };
    }
}
