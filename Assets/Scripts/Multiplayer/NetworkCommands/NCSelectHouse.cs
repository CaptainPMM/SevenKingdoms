namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSelectHouse : NetworkCommand {
            public int selectedHouseTypeInt;

            public override int type {
                get {
                    return (int)NCType.SELECT_HOUSE;
                }
            }

            public NCSelectHouse(HouseType houseType) {
                selectedHouseTypeInt = (int)houseType;
            }
        }
    }
}