using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer {
    public class Server : MonoBehaviour {
        public static Server instance;

        private List<TcpClient> clients;
        private List<Thread> clientStreamThreads;

        private TcpListener listener;
        private Thread receiveFirstRequestsThread;

        private void Start() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            // After start wait for clients to connect to this server
            receiveFirstRequestsThread = new Thread(new ThreadStart(ReceiveFirstRequests));
            receiveFirstRequestsThread.IsBackground = true;
            receiveFirstRequestsThread.Start();
        }

        private void ReceiveFirstRequests() {
            listener = new TcpListener(IPAddress.Any, 4242);
            listener.Start();

            clients = new List<TcpClient>();
            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
            }
        }

        public void StartGame() {
            receiveFirstRequestsThread.Abort();

            // Now listen for game info from the clients
            clientStreamThreads = new List<Thread>();
            foreach (TcpClient client in clients) {
                Thread t = new Thread(() => ReceiveGameInfos(client));
                t.IsBackground = true;
                t.Start();
                clientStreamThreads.Add(t);
            }
        }

        private void ReceiveGameInfos(TcpClient client) {
            NetworkStream ns = client.GetStream();

            // Inform clients of game start
            // TODO...

            byte[] data = new byte[1024];
            while (client.Connected) {
                if (ns.Read(data, 0, data.Length) > 0) {
                    string msg = Encoding.UTF8.GetString(data);
                    Debug.Log(msg);
                }
            }

            clients.Remove(client);
        }

        public void Send(Object stuff) {
            // TODO...
            Debug.LogWarning("Server.Send is TODO");
        }

        private void OnApplicationQuit() {
            receiveFirstRequestsThread.Abort();

            foreach (Thread t in clientStreamThreads) {
                t.Abort();
            }
        }
    }
}