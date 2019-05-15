using System;
using UnityEngine;

[System.Serializable]
public class Building {
    public BuildingType buildingType;

    [HideInInspector]
    public string buildingName {
        get {
            return CreateBuildingInstance(buildingType)._buildingName;
        }
    }

    [HideInInspector]
    public string description {
        get {
            return CreateBuildingInstance(buildingType)._description;
        }
    }

    [HideInInspector]
    public int neededGold {
        get {
            return CreateBuildingInstance(buildingType)._neededGold;
        }
    }

    [HideInInspector]
    public GameEffect[] gameEffects {
        get {
            return CreateBuildingInstance(buildingType)._gameEffects;
        }
    }

    protected string _buildingName;
    protected string _description;
    protected int _neededGold;
    protected GameEffect[] _gameEffects;

    public static Building CreateBuildingInstance(BuildingType buildingType) {
        switch (buildingType) {
            case BuildingType.LOCAL_ADMINISTRATION:
                return new LocalAdministration();
            case BuildingType.MARKETPLACE:
                return new Marketplace();
            case BuildingType.OUTER_TOWN_RING:
                return new OuterTownRing();
            case BuildingType.WOODEN_WALL:
                return new WoodenWall();
            case BuildingType.STONE_WALL:
                return new StoneWall();
            case BuildingType.ADVANCED_WALL:
                return new AdvancedWall();

            default:
                throw new Exception("Invalid BuildingType <" + buildingType + ">: could not be found");
        }
    }

    public static BuildingType[] CreateBuildingTypesArray() {
        return (BuildingType[])Enum.GetValues(typeof(BuildingType));
    }

    /**
        Duplicate for CreateBuildingInstance, but the name might be better for use in other classes in some cases
     */
    public static Building GetBuildingTypeInfos(BuildingType buildingType) {
        return CreateBuildingInstance(buildingType);
    }
}