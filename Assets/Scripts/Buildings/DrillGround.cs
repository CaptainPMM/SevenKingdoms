public class DrillGround : Building {
    public DrillGround() {
        buildingType = BuildingType.DRILL_GROUND;
        _buildingName = "Drill Ground";
        _description = "A spacious yard to train new recruits. Increases the recruitment speed a lot.";
        _neededGold = 2250;
        _gameEffects = new GameEffect[] {
            GameEffect.RECRUITMENT_SPEED_MODIFIER_HIGH
        };
    }
}