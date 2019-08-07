namespace Multiplayer {
    namespace NetworkCommands {
        public class NCRecruit : NetworkCommand {
            public string locationName;
            public int[] soldierNums;

            public NCRecruit(GameLocation location, Soldiers soldiers) {
                type = (int)NCType.RECRUIT;
                this.locationName = location.name;
                soldierNums = SoldiersObjToNumsArray(soldiers);
            }
        }
    }
}