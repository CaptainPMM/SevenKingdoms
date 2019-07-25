using UnityEngine;

public class CameraControllerLoader : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // Set camera postition to player locations
        GameObject[] locations = GameObject.FindGameObjectsWithTag("game_location");
        HouseType playerHouseType = GameController.activeGameController.player.GetComponent<GamePlayer>().house.houseType;
        foreach (GameObject location in locations) {
            GameLocation gl = location.GetComponent<GameLocation>();
            if (gl.house.houseType == playerHouseType) {
                Vector3 camPos = gameObject.transform.position;
                camPos.x = gl.transform.position.x;
                camPos.z = gl.transform.position.z;
                gameObject.transform.position = camPos;
                break;
            }
        }

        // Load appropriate CameraController Component (Script) for Mobile or PC
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            gameObject.AddComponent<CameraControllerMOBILE>();
        } else {
            gameObject.AddComponent<CameraControllerPC>();
        }

        Destroy(this);
    }
}
