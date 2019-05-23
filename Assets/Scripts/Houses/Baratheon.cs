using UnityEngine;

public class Baratheon : House {
    public Baratheon() {
        _houseName = "Baratheon";
        _color = new Color32(231, 202, 146, 255);
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
