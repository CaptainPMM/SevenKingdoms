using UnityEngine;

public class Tully : House {
    public Tully() {
        _houseName = "Tully";
        _color = new Color32(232, 94, 70, 255);
        _houseFlag = Resources.Load<Sprite>(HOUSE_FLAGS_PATH + _houseName);
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.WOODEN_WALL,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL,
            BuildingType.WOOD_MILL,
            BuildingType.BOW_MAKER,
            BuildingType.BLACKSMITH,
            BuildingType.BARRACKS,
            BuildingType.DRILL_GROUND
        };
    }
}
