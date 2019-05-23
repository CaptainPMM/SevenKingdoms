public class Marketplace : Building {
    public Marketplace() {
        buildingType = BuildingType.MARKETPLACE;
        _buildingName = "Marketplace";
        _description = "A place where your citiziens can sell and buy goods. Increases Gold income.";
        _neededGold = 1000;
        _gameEffects = new GameEffect[] {
            GameEffect.GOLD_INCOME_MODIFIER_LOW
         };
    }
}