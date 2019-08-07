namespace Multiplayer {
    namespace NetworkCommands {
        public class NCBeginGame : NetworkCommand {
            public NCBeginGame() {
                type = (int)NCType.BEGIN_GAME;
            }
        }
    }
}