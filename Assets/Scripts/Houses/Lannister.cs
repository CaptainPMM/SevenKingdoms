using UnityEngine;

public class Lannister : House {
    public Lannister() {
        _houseName = "Lannister";
        _color = new Color32(155, 29, 30, 255);
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
