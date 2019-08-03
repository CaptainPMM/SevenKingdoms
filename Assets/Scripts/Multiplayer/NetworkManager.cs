using UnityEngine;

namespace Multiplayer {
    public class NetworkManager : MonoBehaviour {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.S)) {
                // Start server
                gameObject.AddComponent<Server>();
                Destroy(this);
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                // Start Client
                gameObject.AddComponent<Client>();
                Destroy(this);
            }
        }
    }
}