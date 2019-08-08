using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Multiplayer {
    public class Client : MonoBehaviour {
        public static Client instance;

        private string serverIP;

        public TcpClient server;
        private Thread receiveThread;

        public static event NetworkManager.ConnectionEstablished OnConnectionEstablished;
        public static event NetworkManager.ConnectionFailed OnConnectionFailed;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void ConnectToServer(string hostIP) {
            serverIP = hostIP;
            receiveThread = new Thread(new ThreadStart(ListenForData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ListenForData() {
            try {
                // Search server in LAN...
                try {
                    server = new TcpClient(serverIP, 4242);
                    if (!server.Connected) throw new System.Exception();
                    NetworkManager.mpActions.Enqueue(() => OnConnectionEstablished());
                } catch (System.Exception e) {
                    Debug.LogError("Error while connecting to server: " + e);
                    NetworkManager.mpActions.Enqueue(() => OnConnectionFailed());
                    StopClient();
                    return; // Stop thread
                }

                NetworkManager.ListenForNetworkData(server); // Blocks until connection closed

                Debug.LogWarning("Server connection closed");
                StopClient();
            } catch (ThreadAbortException) { }
        }

        private void StopClient() {
            NetworkManager.mpActions.Enqueue(() => {
                Destroy(this);
            });
        }

        private void OnDestroy() {
            if (server != null) server.Close();
            if (receiveThread != null) receiveThread.Abort();
            instance = null;
            NetworkManager.mpActions.Enqueue(() => {
                NetworkManager.mpActive = false;
                OnConnectionFailed();
            });
        }
    }
}