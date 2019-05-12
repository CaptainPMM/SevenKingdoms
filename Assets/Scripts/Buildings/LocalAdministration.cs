public class LocalAdministration : Building {
    public LocalAdministration() {
        _buildingName = "Local Administration";
        _description = "Every town begins with a Local Administration. Enables recruitment of Conscripts and basic town effects.";
        _gameEffects = new GameEffect[] {
            GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER // TEST
        };
    }
}