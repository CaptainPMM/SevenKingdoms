public class Blacksmith : Building {
    public Blacksmith() {
        buildingType = BuildingType.BLACKSMITH;
        _buildingName = "Blacksmith";
        _description = "Craft some proper weapons with an experienced blacksmith. Required for recruitment of Swordsmen and Mounted Knights.";
        _neededGold = 900;
        _gameEffects = new GameEffect[] { };
    }
}