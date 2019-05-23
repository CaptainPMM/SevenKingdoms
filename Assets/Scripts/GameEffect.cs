using UnityEngine;

[System.Serializable]
public class GameEffect {
    // ***************** //
    // Available Effects //
    // ***************** //

    public static GameEffect LOCATION_DEFENDER_CASUALTIES_MODIFIER_LOW = new GameEffect
    (
        "LOCATION_DEFENDER_CASUALTIES_MODIFIER_LOW",
        GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS,
        0.85f
    );
    public static GameEffect LOCATION_DEFENDER_CASUALTIES_MODIFIER_MED = new GameEffect
    (
        "LOCATION_DEFENDER_CASUALTIES_MODIFIER_MED",
        GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS,
        0.75f
    );
    public static GameEffect LOCATION_DEFENDER_CASUALTIES_MODIFIER_HIGH = new GameEffect
    (
        "LOCATION_DEFENDER_CASUALTIES_MODIFIER_HIGH",
        GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS,
        0.65f
    );

    public static GameEffect GOLD_INCOME_MODIFIER_LOW = new GameEffect
    (
        "GOLD_INCOME_MODIFIER_LOW",
        GameEffectType.GOLD_INCOME,
        1.1f
    );
    public static GameEffect GOLD_INCOME_MODIFIER_MED = new GameEffect
    (
        "GOLD_INCOME_MODIFIER_MED",
        GameEffectType.GOLD_INCOME,
        1.2f
    );
    public static GameEffect GOLD_INCOME_MODIFIER_HIGH = new GameEffect
    (
        "GOLD_INCOME_MODIFIER_HIGH",
        GameEffectType.GOLD_INCOME,
        1.3f
    );

    public static GameEffect MANPOWER_INCOME_MODIFIER_LOW = new GameEffect
    (
        "MANPOWER_INCOME_MODIFIER_LOW",
        GameEffectType.MANPOWER_INCOME,
        1.1f
    );
    public static GameEffect MANPOWER_INCOME_MODIFIER_HIGH = new GameEffect
    (
        "MANPOWER_INCOME_MODIFIER_HIGH",
        GameEffectType.MANPOWER_INCOME,
        1.2f
    );

    // *********************** //
    // -- Available Effects -- //
    // *********************** //

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
