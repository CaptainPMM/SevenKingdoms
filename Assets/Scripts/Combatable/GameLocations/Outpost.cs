using UnityEngine;

public class Outpost : GameLocation {
    // Start is called before the first frame update
    new void Start() {
        base.Start();

        BASE_GOLD_INCOME = 2;
        BASE_MANPOWER_INCOME = 4;

        buildableBuildings.AddRange(new BuildingType[] {
            BuildingType.LOCAL_ADMINISTRATION,
            BuildingType.WOODEN_WALL
        });

        buildings.Add(new LocalAdministration());
        GetEffectsFromBuildings();
    }
}
