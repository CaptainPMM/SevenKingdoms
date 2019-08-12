namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSyncCombat : NetworkCommand {
            public string fightingHouseID;
            public int soldierTypeInt;
            public int damage;

            public override int type {
                get {
                    return (int)NCType.SYNC_COMBAT;
                }
            }

            public NCSyncCombat(FightingHouse fightingHouse, SoldierType soldierType, int damage) {
                fightingHouseID = fightingHouse.ID;
                soldierTypeInt = (int)soldierType;
                this.damage = damage;
            }
        }
    }
}