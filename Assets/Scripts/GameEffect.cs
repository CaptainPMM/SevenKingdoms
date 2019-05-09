using UnityEngine;

[System.Serializable]
public class GameEffect {
    // Available Effects
    public static GameEffect LOCATION_DEFENDER_CASUALTIES_MODIFIER = new GameEffect
    (
        "LOCATION_DEFENDER_CASUALTIES_MODIFIER",
        GameEffectType.GAME_LOCATION,
        0.66f
    );
    // --

    public string name {
        get {
            return _name;
        }
    }
    public GameEffectType type {
        get {
            return _type;
        }
    }
    public float modifierValue {
        get {
            return _modifierValue;
        }
    }

    [SerializeField] private string _name;
    [SerializeField] private GameEffectType _type;
    [SerializeField] private float _modifierValue;

    private GameEffect(string name, GameEffectType type, float modifierValue) {
        _name = name;
        _type = type;
        _modifierValue = modifierValue;
    }
}
