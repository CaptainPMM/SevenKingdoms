public class Blacksmith : Building {
    public Blacksmith() {
        buildingType = BuildingType.BLACKSMITH;
        _buildingName = "Blacksmith";
        _description = "Craft some proper weapons with an experienced blacksmith. Required for recruitment of Swordsmen and Mounted Knights.";
        _neededGold = 500;
        _gameEffects = new GameEffect[] {
            GameEffect.ST_UNLOCK_SWORDSMEN,
            GameEffect.ST_UNLOCK_MOUNTED_KNIGHTS
        };
    }
}