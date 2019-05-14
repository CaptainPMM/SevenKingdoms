public class WoodenWall : Building {
    public WoodenWall() {
        buildingType = BuildingType.WOODEN_WALL;
        _buildingName = "Wooden Wall";
        _description = "The outpost defenses have to be improved. Increases defense bonus.";
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER
        };
    }
}