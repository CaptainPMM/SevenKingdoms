using UnityEngine;

public class Neutral : House {
    public Neutral() {
        _houseName = "Neutral";
        _color = new Color32(150, 150, 150, 255);
        _buildableBuildings = new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION
        };
    }
}
