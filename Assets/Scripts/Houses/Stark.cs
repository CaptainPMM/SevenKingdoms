using UnityEngine;

public class Stark : House {
    public Stark() {
        _houseName = "Stark";
        _color = new Color32(230, 230, 240, 255);
        _buildableBuildings = new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION
        };
    }
}
