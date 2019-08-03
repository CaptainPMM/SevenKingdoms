using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Multiplayer {
    public class Client : MonoBehaviour {
        public static Client instance;

        private TcpClient server;
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
            server = new TcpClient("localhost", 4242); // TODO...
        }

        public void Send(Object stuff) {
            // TODO...
            Debug.LogWarning("Server.Send is TODO");
        }

        private void OnApplicationQuit() {
            receiveThread.Abort();
        }
    }
}