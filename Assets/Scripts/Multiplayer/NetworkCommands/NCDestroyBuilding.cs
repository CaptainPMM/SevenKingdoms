namespace Multiplayer {
    namespace NetworkCommands {
        public class NCDestroyBuilding : NetworkCommand {
            public string locationName;
            public int buildingTypeInt;

            public override int type {
                get {
                    return (int)NCType.DESTROY_BUILDING;
                }
            }

            public NCDestroyBuilding(GameLocation location, BuildingType buildingType) {
                this.locationName = location.name;
                this.buildingTypeInt = (int)buildingType;
            }
        }
    }
}