using UnityEngine;

public class Tyrell : House {
    public Tyrell() {
        _houseName = "Tyrell";
        _color = new Color32(10, 200, 20, 255);
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
