public class Barracks : Building {
    public Barracks() {
        buildingType = BuildingType.BARRACKS;
        _buildingName = "Barracks";
        _description = "A basic place to train new soldiers. Increases recruitment speed a bit.";
        _neededGold = 150;
        _gameEffects = new GameEffect[] {
            GameEffect.RECRUITMENT_SPEED_MODIFIER_LOW
        };
    }
}