using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class House {
    private static Dictionary<HouseType, House> houseInstances;

    public HouseType houseType;

    public House() { }
    public House(HouseType ht) {
        houseType = ht;
    }

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
    public Sprite houseFlag {
        get {
            return GetHouse()._houseFlag;
        }
    }
    [HideInInspector]
    public List<BuildingType> buildableBuildings {
        get {
            return GetHouse()._buildableBuildings;
        }
    }

    [HideInInspector]
    public int gold {
        get {
            return GetHouse()._gold;
        }
        set {
            GetHouse()._gold = value;
        }
    }
    [HideInInspector]
    public int manpower {
        get {
            return GetHouse()._manpower;
        }
        set {
            GetHouse()._manpower = value;
        }
    }

    protected string _houseName;
    protected Color _color;
    protected const string HOUSE_FLAGS_PATH = "HouseFlags/";
    protected Sprite _houseFlag;
    protected List<BuildingType> _buildableBuildings;

    protected int _gold;
    protected int _manpower;

    private House GetHouse() {
        if (houseInstances == null) {
            houseInstances = new Dictionary<HouseType, House>() {
                { HouseType.NEUTRAL, new Neutral() },
                { HouseType.STARK, new Stark() },
                { HouseType.TULLY, new Tully() },
                { HouseType.ARRYN, new Arryn() },
                { HouseType.LANNISTER, new Lannister() },
                { HouseType.BARATHEON, new Baratheon() },
                { HouseType.TYRELL, new Tyrell() },
                { HouseType.MARTELL, new Martell() }
            };
        }

        return houseInstances[houseType];
    }
}
