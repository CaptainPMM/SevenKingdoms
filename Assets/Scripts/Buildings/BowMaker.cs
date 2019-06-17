public class BowMaker : Building {
    public BowMaker() {
        buildingType = BuildingType.BOW_MAKER;
        _buildingName = "Bow Maker";
        _description = "Transforms the processed wood from the wood mill into bows. Enables recruitment of Bowmen.";
        _neededGold = 750;
        _gameEffects = new GameEffect[] {
            GameEffect.ST_UNLOCK_BOWMEN
        };
    }
}