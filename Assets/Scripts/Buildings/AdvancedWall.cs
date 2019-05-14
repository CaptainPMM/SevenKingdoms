public class AdvancedWall : Building {
    public AdvancedWall() {
        buildingType = BuildingType.ADVANCED_WALL;
        _buildingName = "Advanced Wall";
        _description = "Further increase the defender bonus of the castle by building a stronger perimeter wall.";
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER
        };
    }
}