using UnityEngine;

public class Stark : House {
    public Stark() {
        _houseName = "Stark";
        _color = new Color32(232, 225, 197, 255);
        _houseFlag = Resources.Load<Sprite>(HOUSE_FLAGS_PATH + _houseName);
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.MARKETPLACE,
            BuildingType.OUTER_TOWN_RING,
            BuildingType.WOODEN_WALL,
            BuildingType.STONE_WALL,
            BuildingType.ADVANCED_WALL,
            BuildingType.BLACKSMITH,
            BuildingType.STABLES
        };
    }
}
