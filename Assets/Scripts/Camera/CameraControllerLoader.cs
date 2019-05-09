using UnityEngine;

public class CameraControllerLoader : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // Load appropriate CameraController Component (Script) for Mobile or PC
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            gameObject.AddComponent<CameraControllerMOBILE>();
        } else {
            gameObject.AddComponent<CameraControllerPC>();
        }

        Destroy(this);
    }
}
