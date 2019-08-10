namespace Multiplayer {
    namespace NetworkCommands {
        public class NCBuild : NetworkCommand {
            public string locationName;
            public int buildingTypeInt;

            public override int type {
                get {
                    return (int)NCType.BUILD;
                }
            }

            public NCBuild(GameLocation location, BuildingType buildingType) {
                this.locationName = location.name;
                this.buildingTypeInt = (int)buildingType;
            }
        }
    }
}