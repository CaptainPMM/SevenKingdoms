using UnityEngine;

[System.Serializable]
public class GameEffect {

    // ***************** //
    // Available Effects //
    // ***************** //

    // ### COMBAT_LOCATION_DEFENDER_BONUS ###

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

    // ### GOLD_INCOME ###

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

    // ### MANPOWER_INCOME ###

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

    // ### SOLDIER_TYPE_UNLOCK ###

    public static GameEffect ST_UNLOCK_CONSCRIPTS = new GameEffect
    (
        "ST_UNLOCK_CONSCRIPTS",
        GameEffectType.SOLDIER_TYPE_UNLOCK,
        0
    );
    public static GameEffect ST_UNLOCK_SPEARMEN = new GameEffect
    (
        "ST_UNLOCK_SPEARMEN",
        GameEffectType.SOLDIER_TYPE_UNLOCK,
        1
    );
    public static GameEffect ST_UNLOCK_SWORDSMEN = new GameEffect
    (
        "ST_UNLOCK_SWORDSMEN",
        GameEffectType.SOLDIER_TYPE_UNLOCK,
        2
    );
    public static GameEffect ST_UNLOCK_BOWMEN = new GameEffect
    (
        "ST_UNLOCK_BOWMEN",
        GameEffectType.SOLDIER_TYPE_UNLOCK,
        3
    );
    public static GameEffect ST_UNLOCK_MOUNTED_KNIGHTS = new GameEffect
    (
        "ST_UNLOCK_MOUNTED_KNIGHTS",
        GameEffectType.SOLDIER_TYPE_UNLOCK,
        4
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
