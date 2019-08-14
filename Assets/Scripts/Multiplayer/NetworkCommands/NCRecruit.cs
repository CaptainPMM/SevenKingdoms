namespace Multiplayer {
    namespace NetworkCommands {
        public class NCRecruit : NetworkCommand {
            public string locationName;
            public int[] soldierNums;

            public override int type {
                get {
                    return (int)NCType.RECRUIT;
                }
            }

            public NCRecruit(GameLocation location, Soldiers soldiers) {
                this.locationName = location.name;
                soldierNums = SoldiersObjToNumsArray(soldiers);
            }
        }
    }
}