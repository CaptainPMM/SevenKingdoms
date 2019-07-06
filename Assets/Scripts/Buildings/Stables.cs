public class Stables : Building {
    public Stables() {
        buildingType = BuildingType.STABLES;
        _buildingName = "Stables";
        _description = "Build a place for horses. Enables recruitment of Mounted Knights.";
        _neededGold = 900;
        _gameEffects = new GameEffect[] {
            GameEffect.ST_UNLOCK_MOUNTED_KNIGHTS
        };
    }
}