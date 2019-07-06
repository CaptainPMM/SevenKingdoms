public class WoodMill : Building {
    public WoodMill() {
        buildingType = BuildingType.WOOD_MILL;
        _buildingName = "Wood Mill";
        _description = "Used to process raw wood. Required for the recruitment of Spearmen and Bowmen.";
        _neededGold = 200;
        _gameEffects = new GameEffect[] {
            GameEffect.ST_UNLOCK_SPEARMEN,
            GameEffect.ST_UNLOCK_BOWMEN
        };
    }
}