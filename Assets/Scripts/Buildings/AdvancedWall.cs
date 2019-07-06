public class AdvancedWall : Building {
    public AdvancedWall() {
        buildingType = BuildingType.ADVANCED_WALL;
        _buildingName = "Advanced Wall";
        _description = "Further increase to the defender bonus of the castle by building a stronger perimeter wall.";
        _neededGold = 1500;
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER_HIGH
        };
    }
}