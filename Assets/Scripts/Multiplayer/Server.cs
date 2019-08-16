using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer {
    public class Server : MonoBehaviour {
        public static Server instance;

        private bool inGame;

        private TcpListener connectionListener;
        private Thread connectionListenerThread;

        public List<TcpClient> clients;
        public Dictionary<TcpClient, HouseType> clientHouseTypes;
        public TcpClient serverTCPClientObject;
        private List<Thread> clientStreamThreads;

        public static event NetworkManager.ConnectionEstablished OnConnectionEstablished;
        public static event NetworkManager.ConnectionFailed OnConnectionFailed;

        public static event NetworkManager.ConnectionEstablished OnClientConnect;
        public static event NetworkManager.ConnectionFailed OnClientDisconnect;

        private TMPro.TextMeshProUGUI hostStatusText;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            clients = new List<TcpClient>();
            clientHouseTypes = new Dictionary<TcpClient, HouseType>();
            clientStreamThreads = new List<Thread>();

            serverTCPClientObject = new TcpClient();

            clientHouseTypes.Add(serverTCPClientObject, HouseSelMenu.UISelectedHouseType);

            hostStatusText = GameObject.Find("Text Host Status").GetComponent<TMPro.TextMeshProUGUI>();

            // After start wait for clients to connect to this server
            connectionListenerThread = new Thread(new ThreadStart(ListenForConnections));
            connectionListenerThread.IsBackground = true;
            connectionListenerThread.Start();
        }

        private void ListenForConnections() {
            inGame = false;
            try {
                connectionListener = new TcpListener(IPAddress.Any, 4242);
                connectionListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                connectionListener.Start();
                NetworkManager.mpActions.Enqueue(() => OnConnectionEstablished());

                while (true) {
                    TcpClient client = connectionListener.AcceptTcpClient();
                    if (clients.Count < System.Enum.GetValues(typeof(HouseType)).Length - 2) { // - 2 for the neutral house (not playable) and server player
                        clients.Add(client);

                        Thread t = new Thread(() => ListenForData(client));
                        t.IsBackground = true;
                        t.Start();
                        clientStreamThreads.Add(t);

                        // Inform UI of new connection
                        NetworkManager.mpActions.Enqueue(() => {
                            hostStatusText.text = "Hosting (" + clients.Count + " connected)";
                            OnClientConnect();
                        });
                    } else {
                        // Player limit reached -> reject
                        client.Close();
                    }
                }
            } catch (System.Exception e) {
                if (e.GetType() == typeof(SocketException)) {
                    if (((SocketException)e).Message == "interrupted") return; // Stop thread before next exception check (iOS only)
                }
                if (e.GetType() != typeof(ThreadAbortException)) {
                    Debug.LogError("Error while listening for connections: " + e);
                    NetworkManager.mpActions.Enqueue(() => OnConnectionFailed());
                    StopServer();
                }
                return; // Stop thread
            }
        }

        private void ListenForData(TcpClient client) {
            try {
                NetworkManager.ListenForNetworkData(client); // Blocks until connection closed

                Debug.LogWarning("Client connection lost");
                client.Close();
                clients.Remove(client);
                clientStreamThreads.Remove(Thread.CurrentThread);
                RemoveClientHouseType(client);
                // Inform UI of lost connection
                NetworkManager.mpActions.Enqueue(() => {
                    if (!inGame) hostStatusText.text = "Hosting (" + clients.Count + " connected)";
                    OnClientDisconnect();
                });
                if (inGame && clients.Count < 1) {
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

        public void StartGame() {
            inGame = true;

            connectionListener.Stop();
            connectionListenerThread.Abort();

            // Inform clients of game start
            NetworkManager.Send(new NetworkCommands.NCBeginGame());
        }

        public void RemoveClientHouseType(TcpClient client) {
            HouseType houseType;
            if (clientHouseTypes.TryGetValue(client, out houseType)) {
                clientHouseTypes.Remove(client);
            }
        }

        private void StopServer() {
            NetworkManager.mpActions?.Enqueue(() => {
                Destroy(this);
            });
        }

        private void OnDestroy() {
            if (connectionListenerThread != null) connectionListenerThread.Abort();

            foreach (Thread t in clientStreamThreads) t.Abort();

            foreach (TcpClient c in clients) c.Close();

            instance = null;

            NetworkManager.mpActions?.Enqueue(() => {
                NetworkManager.isServer = false;
                NetworkManager.mpActive = false;
            });
        }
    }
}