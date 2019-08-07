namespace Multiplayer {
    namespace NetworkCommands {
        public class NCBuild : NetworkCommand {
            public string locationName;
            public int buildingTypeInt;

            public NCBuild(GameLocation location, BuildingType buildingType) {
                type = (int)NCType.BUILD;
                this.locationName = location.name;
                this.buildingTypeInt = (int)buildingType;
            }
        }
    }
}