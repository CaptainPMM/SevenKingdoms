namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSelectHouseReq : NetworkCommand {
            public int houseTypeInt;
            public bool init;

            public override int type {
                get {
                    return (int)NCType.SELECT_HOUSE_REQUEST;
                }
            }

            public NCSelectHouseReq(HouseType houseType, bool initRequest = false) {
                houseTypeInt = (int)houseType;
                init = initRequest;
            }
        }
    }
}