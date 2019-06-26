using UnityEngine;

public class Baratheon : House {
    public Baratheon() {
        _houseName = "Baratheon";
        _color = new Color32(254, 207, 3, 255);
        _houseFlag = Resources.Load<Sprite>(HOUSE_FLAGS_PATH + _houseName);
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.WOODEN_WALL,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL,
            BuildingType.BLACKSMITH,
            BuildingType.STABLES,
            BuildingType.BARRACKS,
            BuildingType.DRILL_GROUND
        };
    }
}
