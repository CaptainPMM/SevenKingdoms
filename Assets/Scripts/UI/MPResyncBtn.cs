using UnityEngine;

public class MPResyncBtn : MonoBehaviour {
    public void ResyncBtnClicked() {
        string saveGame = GameController.activeGameController.FastSaveGame();
        Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncGame(false, saveGame));
        GameController.activeGameController.HandleFastSaveGameData(saveGame);
    }
}
