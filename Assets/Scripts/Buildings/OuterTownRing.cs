public class OuterTownRing : Building {
    public OuterTownRing() {
        buildingType = BuildingType.OUTER_TOWN_RING;
        _buildingName = "Outer Town Ring";
        _description = "The town has to grow. Build an outer town ring around the perimeter. Increases Manpower income.";
        _neededGold = 600;
        _gameEffects = new GameEffect[] {
            GameEffect.MANPOWER_INCOME_MODIFIER_HIGH
         };
    }
}