using UnityEngine;

public class Lennister : House {
    public Lennister() {
        _houseName = "Lennister";
        _color = new Color32(200, 0, 0, 255);
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
