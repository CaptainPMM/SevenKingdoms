using UnityEngine;

public class Martell : House {
    public Martell() {
        _houseName = "Martell";
        _color = new Color32(255, 154, 78, 255);
        _houseFlag = Resources.Load<Sprite>(HOUSE_FLAGS_PATH + _houseName);
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.WOODEN_WALL,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL,
            BuildingType.WOOD_MILL,
            BuildingType.DRILL_GROUND
        };
    }
}
