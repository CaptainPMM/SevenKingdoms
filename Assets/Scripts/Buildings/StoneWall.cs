public class StoneWall : Building {
    public StoneWall() {
        buildingType = BuildingType.STONE_WALL;
        _buildingName = "Stone Wall";
        _description = "Every castle already starts with a stone wall. Enables defender bonus.";
        _neededGold = 0;
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER
        };
    }
}