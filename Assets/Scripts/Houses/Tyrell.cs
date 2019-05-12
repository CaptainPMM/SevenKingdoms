using UnityEngine;

public class Tyrell : House {
    public Tyrell() {
        _houseName = "Tyrell";
        _color = new Color32(10, 200, 20, 255);
        _buildableBuildings = new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION
        };
    }
}
