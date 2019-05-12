﻿using UnityEngine;

public class Lennister : House {
    public Lennister() {
        _houseName = "Lennister";
        _color = new Color32(200, 0, 0, 255);
        _buildableBuildings = new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION
        };
    }
}
