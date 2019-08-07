namespace Multiplayer {
    namespace NetworkCommands {
        public class NCMoveTroops : NetworkCommand {
            public string originLocationName;
            public string destLocationName;
            public int[] soldierNums;

            /// <summary>Send all soldiers</summary>
            public NCMoveTroops(GameLocation originLocation, GameLocation destLocation) {
                type = (int)NCType.MOVE_TROOPS;
                this.originLocationName = originLocation.name;
                this.destLocationName = destLocation.name;
                soldierNums = null;
            }

            /// <summary>Only send specific soldiers</summary>
            public NCMoveTroops(GameLocation originLocation, GameLocation destLocation, Soldiers soldiers) {
                type = (int)NCType.MOVE_TROOPS;
                this.originLocationName = originLocation.name;
                this.destLocationName = destLocation.name;
                soldierNums = SoldiersObjToNumsArray(soldiers);
            }
        }
    }
}