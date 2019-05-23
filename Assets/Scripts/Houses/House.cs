﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class House {
    public HouseType houseType;

    [HideInInspector]
    public string houseName {
        get {
            return GetHouse()._houseName;
        }
    }
    [HideInInspector]
    public Color color {
        get {
            return GetHouse()._color;
        }
    }
    [HideInInspector]
    public List<BuildingType> buildableBuildings {
        get {
            return GetHouse()._buildableBuildings;
        }
    }

    protected string _houseName;
    protected Color _color;
    protected List<BuildingType> _buildableBuildings;

    private House GetHouse() {
        switch (houseType) {
            case HouseType.NEUTRAL:
                return new Neutral();
            case HouseType.STARK:
                return new Stark();
            case HouseType.TULLY:
                return new Tully();
            case HouseType.ARRYN:
                return new Arryn();
            case HouseType.LANNISTER:
                return new Lannister();
            case HouseType.BARATHEON:
                return new Baratheon();
            case HouseType.TYRELL:
                return new Tyrell();
            case HouseType.MARTELL:
                return new Martell();

            default:
                return new Neutral();
        }
    }
}
