public enum BuildingType {
    LOCAL_ADMINISTRATION, // enables conscripts and basic game location features // always pre-built
    MARKETPLACE, // increased taxes -> more gold output
    OUTER_TOWN_RING, // increases manpower
    WOODEN_WALL, // basic defender bonus // builable in outposts // not in castles -> stone wall is prebuilt
    STONE_WALL, // better defender bonus // pre-built in castles
    ADVANCED_WALL, // best defender bonus
    WOOD_MILL, // requirement for spearmen and bowmen
    BOW_MAKER, // requirement for bowmen
    BLACKSMITH, // requirement for swordsmen and mounted knights
    STABLES // requirement for mounted knights
}