using UnityEngine;

public class Lannister : House {
    public Lannister() {
        _houseName = "Lannister";
        _color = new Color32(153, 9, 9, 255);
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
            BuildingType.STABLES,
            BuildingType.BARRACKS
        };
    }
}
