public class LocalAdministration : Building {
    public LocalAdministration() {
        buildingType = BuildingType.LOCAL_ADMINISTRATION;
        _buildingName = "Local Administration";
        _description = "Every town begins with a Local Administration. Enables recruitment of Conscripts and basic town effects.";
        _neededGold = 0;
        _gameEffects = new GameEffect[] { };
    }
}