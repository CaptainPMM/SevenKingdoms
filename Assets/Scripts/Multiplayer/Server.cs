using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer {
    public class Server : MonoBehaviour {
        public static Server instance;

        private TcpListener connectionListener;
        private Thread connectionListenerThread;

        private Thread connectionTesterThread;

        public List<TcpClient> clients;
        private List<Thread> clientStreamThreads;

        public static event NetworkManager.ConnectionEstablished OnConnectionEstablished;
        public static event NetworkManager.ConnectionFailed OnConnectionFailed;

        private TMPro.TextMeshProUGUI hostStatusText;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            clients = new List<TcpClient>();
            clientStreamThreads = new List<Thread>();

            hostStatusText = GameObject.Find("Text Host Status").GetComponent<TMPro.TextMeshProUGUI>();

            // After start wait for clients to connect to this server
            connectionListenerThread = new Thread(new ThreadStart(ListenForConnections));
            connectionListenerThread.IsBackground = true;
            connectionListenerThread.Start();
        }

        private void ListenForConnections() {
            try {
                connectionListener = new TcpListener(IPAddress.Any, 4242);
                connectionListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                connectionListener.Start();
                NetworkManager.mpActions.Enqueue(() => OnConnectionEstablished());

                connectionTesterThread = new Thread(new ThreadStart(TestConnections));
                connectionTesterThread.IsBackground = true;
                connectionTesterThread.Start();

                while (true) {
                    TcpClient client = connectionListener.AcceptTcpClient();
                    if (clients.Count < System.Enum.GetValues(typeof(HouseType)).Length - 1) { // - 1 for the neutral house (not playable)
                        clients.Add(client);

                        // Inform UI of new connection
                        NetworkManager.mpActions.Enqueue(() => {
                            hostStatusText.text = "Hosting (" + clients.Count + " connected)";
                        });
                    } else {
                        // Player limit reached -> reject
                        client.Close();
                    }
                }
            } catch (System.Exception e) {
                if (e.GetType() != typeof(ThreadAbortException)) {
                    Debug.LogError("Error while listening for connections: " + e);
                    NetworkManager.mpActions.Enqueue(() => OnConnectionFailed());
                    StopServer();
                }
                return; // Stop thread
            }
        }

        private void TestConnections() {
            while (!connectionListenerThread.ThreadState.HasFlag(ThreadState.Stopped)) {
                Thread.Sleep(3000);
                foreach (TcpClient c in clients.ToArray()) {
                    try {
                        c.GetStream().WriteByte(0);
                    } catch (System.Exception) {
                        c.Close();
                        clients.Remove(c);
                        // Inform UI of lost connection
                        NetworkManager.mpActions.Enqueue(() => {
                            hostStatusText.text = "Hosting (" + clients.Count + " connected)";
                        });
                    }
                }
            }
        }

        public void StartGame() {
            connectionListener.Stop();
            connectionListenerThread.Abort();

            // Inform clients of game start
            NetworkManager.Send(new NetworkCommands.NCBeginGame());

            // Now listen for game info from the clients
            foreach (TcpClient client in clients) {
                Thread t = new Thread(() => ListenForData(client));
                t.IsBackground = true;
                t.Start();
                clientStreamThreads.Add(t);
            }
        }

        private void ListenForData(TcpClient client) {
            try {
                NetworkManager.ListenForNetworkData(client); // Blocks until connection closed

                Debug.LogWarning("Client connection lost");
                clients.Remove(client);
                clientStreamThreads.Remove(Thread.CurrentThread);
                if (clients.Count < 1) {
                    Debug.LogWarning("No clients connected, stopping server");
                    StopServer();
                }
            } catch (ThreadAbortException) {
                client.Close();
                Debug.LogWarning("Client connection closed");
                clients.Remove(client);
                clientStreamThreads.Remove(Thread.CurrentThread);
                return; // Stop thread
            }
        }

        private void StopServer() {
            NetworkManager.mpActions.Enqueue(() => {
                Destroy(this);
            });
        }

        private void OnDestroy() {
            if (connectionListenerThread != null) connectionListenerThread.Abort();

            foreach (Thread t in clientStreamThreads) t.Abort();

            foreach (TcpClient c in clients) c.Close();

            instance = null;

            NetworkManager.mpActions.Enqueue(() => {
                NetworkManager.isServer = false;
                NetworkManager.mpActive = false;
            });
        }
    }
}