using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Multiplayer {
    public class Client : MonoBehaviour {
        public static Client instance;

        public TcpClient server;
        private Thread receiveThread;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            ConnectToServer();
        }

        private void ConnectToServer() {
            receiveThread = new Thread(new ThreadStart(ListenForData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ListenForData() {
            // Search server in LAN...
            try {
                server = new TcpClient("localhost", 4242); // TODO...
            } catch (System.Exception e) {
                Debug.LogError("Error while connecting to server: " + e);
                StopClient();
                return; // Stop thread
            }

            NetworkManager.ListenForNetworkData(server); // Blocks until connection closed

            Debug.LogWarning("Server connection closed");
            StopClient();
        }

        private void StopClient() {
            server.Close();
            NetworkManager.mpActions.Enqueue(() => {
                NetworkManager.mpActive = false;
                Destroy(this);
            });
        }

        private void OnApplicationQuit() {
            server.Close();
            receiveThread.Abort();
        }
    }
}