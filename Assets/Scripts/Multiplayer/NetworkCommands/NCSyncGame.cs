namespace Multiplayer {
    namespace NetworkCommands {
        public class NCSyncGame : NetworkCommand {
            public bool isRequest;
            public string saveGameData;

            public override int type {
                get {
                    return (int)NCType.SYNC_GAME;
                }
            }

            public NCSyncGame(bool isRequest, string saveGameData = "") {
                this.isRequest = isRequest;
                this.saveGameData = saveGameData;
                if (!isRequest && saveGameData == "") UnityEngine.Debug.LogWarning("NCSyncGame network command needs saveGameData if it is not a request");
            }
        }
    }
}