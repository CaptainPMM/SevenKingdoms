public class WoodenWall : Building {
    public WoodenWall() {
        buildingType = BuildingType.WOODEN_WALL;
        _buildingName = "Wooden Wall";
        _description = "The outpost defenses have to be improved. Enables basic defense bonus.";
        _neededGold = 120;
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER_LOW
        };
    }
}