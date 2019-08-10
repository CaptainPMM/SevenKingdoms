namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSelectHouseRes : NetworkCommand {
            public int houseTypeInt;
            public bool houseTypeIntApproved;
            public bool init;
            public int houseTypeSuggestionInt;

            public override int type {
                get {
                    return (int)NCType.SELECT_HOUSE_RESPONSE;
                }
            }

            public NCSelectHouseRes(HouseType houseType, bool isRequestedHouseTypeApproved, bool initRequest = false, HouseType houseTypeSuggestion = HouseType.NEUTRAL) {
                houseTypeInt = (int)houseType;
                houseTypeIntApproved = isRequestedHouseTypeApproved;
                init = initRequest;
                houseTypeSuggestionInt = (int)houseTypeSuggestion;
            }
        }
    }
}