namespace Multiplayer {
    namespace NetworkCommands {
        public class NCFreeHouse : NetworkCommand {
            public int freeHouseTypeInt;

            public override int type {
                get {
                    return (int)NCType.FREE_HOUSE;
                }
            }

            public NCFreeHouse(HouseType houseType) {
                freeHouseTypeInt = (int)houseType;
            }
        }
    }
}