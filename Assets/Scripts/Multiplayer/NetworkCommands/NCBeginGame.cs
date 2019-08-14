namespace Multiplayer {
    namespace NetworkCommands {
        public class NCBeginGame : NetworkCommand {
            public override int type {
                get {
                    return (int)NCType.BEGIN_GAME;
                }
            }

            public NCBeginGame() { }
        }
    }
}