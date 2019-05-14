public class StoneWall : Building {
    public StoneWall() {
        buildingType = BuildingType.STONE_WALL;
        _buildingName = "Stone Wall";
        _description = "Every castle already has a stone wall. Increases defender bonus.";
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER
        };
    }
}