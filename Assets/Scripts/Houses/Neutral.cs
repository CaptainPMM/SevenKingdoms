using UnityEngine;

public class Neutral : House {
    public Neutral() {
        _houseName = "Neutral";
        _color = new Color32(140, 140, 140, 255);
        _houseFlag = null;
        _buildableBuildings = new System.Collections.Generic.List<BuildingType> { };
    }
}
