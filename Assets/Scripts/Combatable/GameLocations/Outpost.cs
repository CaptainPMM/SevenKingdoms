public class Outpost : GameLocation {
    // Start is called before the first frame update
    new void Start() {
        base.Start();

        recruitmentSpeed = 4f; // in secs

        BASE_GOLD_INCOME = 6;
        BASE_MANPOWER_INCOME = 1;

        buildableBuildings.AddRange(new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.WOODEN_WALL,
            BuildingType.BARRACKS
        });

        buildings.Add(new LocalAdministration());
        GetEffectsFromBuildings();

        DetermineFortificationLevel();
    }
}
