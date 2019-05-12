using System;
using UnityEngine;

[System.Serializable]
public class Building {
    public BuildingType buildingType;

    [HideInInspector]
    public string buildingName {
        get {
            return GetBuilding()._buildingName;
        }
    }

    [HideInInspector]
    public string description {
        get {
            return GetBuilding()._description;
        }
    }

    [HideInInspector]
    public GameEffect[] gameEffects {
        get {
            return GetBuilding()._gameEffects;
        }
    }

    protected string _buildingName;
    protected string _description;
    protected GameEffect[] _gameEffects;

    private Building GetBuilding() {
        switch (buildingType) {
            case BuildingType.LOCAL_ADMINISTRATION:
                return new LocalAdministration();
            default:
                throw new Exception("Invalid BuildingType <" + buildingType + ">: could not be found");
        }
    }
}