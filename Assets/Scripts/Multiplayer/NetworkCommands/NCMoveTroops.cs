namespace Multiplayer {
    namespace NetworkCommands {
        public class NCMoveTroops : NetworkCommand {
            public string originLocationName;
            public string destLocationName;
            public int[] soldierNums;

            public override int type {
                get {
                    return (int)NCType.MOVE_TROOPS;
                }
            }

            /// <summary>Send all soldiers</summary>
            public NCMoveTroops(GameLocation originLocation, GameLocation destLocation) {
                this.originLocationName = originLocation.name;
                this.destLocationName = destLocation.name;
                soldierNums = null;
            }

            /// <summary>Only send specific soldiers</summary>
            public NCMoveTroops(GameLocation originLocation, GameLocation destLocation, Soldiers soldiers) {
                this.originLocationName = originLocation.name;
                this.destLocationName = destLocation.name;
                soldierNums = SoldiersObjToNumsArray(soldiers);
            }
        }
    }
}