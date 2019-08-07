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

        public List<TcpClient> clients;
        private List<Thread> clientStreamThreads;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            clients = new List<TcpClient>();
            clientStreamThreads = new List<Thread>();

            // After start wait for clients to connect to this server
            connectionListenerThread = new Thread(new ThreadStart(ListenForConnections));
            connectionListenerThread.IsBackground = true;
            connectionListenerThread.Start();
        }

        private void ListenForConnections() {
            try {
                connectionListener = new TcpListener(IPAddress.Any, 4242);
                connectionListener.Start();

                while (true) {
                    TcpClient client = connectionListener.AcceptTcpClient();
                    clients.Add(client);
                }
            } catch (System.Exception e) {
                if (e.GetType() != typeof(ThreadAbortException)) {
                    Debug.LogError("Error while listening for connections: " + e);
                    StopServer();
                }
                return; // Stop thread
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
            }
        }

        private void StopServer() {
            NetworkManager.mpActions.Enqueue(() => {
                NetworkManager.isServer = false;
                NetworkManager.mpActive = false;
                Destroy(this);
            });
        }

        private void OnApplicationQuit() {
            connectionListenerThread.Abort();

            foreach (Thread t in clientStreamThreads) {
                t.Abort();
            }
        }
    }
}