namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSyncCombatEnd : NetworkCommand {
            /// <summary>The winner of a combat. It may be that this is not the winner and only one participating fighting house.
            /// This is the case for example if all fighitng houses end with 0 soldiers.</summary>
            public string winnerFightingHouseID;

            public override int type {
                get {
                    return (int)NCType.SYNC_COMBAT_END;
                }
            }

            public NCSyncCombatEnd(FightingHouse winnerFightingHouse) {
                winnerFightingHouseID = winnerFightingHouse.ID;
            }
        }
    }
}