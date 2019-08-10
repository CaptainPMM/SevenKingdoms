using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Multiplayer {
    public class NetworkManager : MonoBehaviour {
        public static NetworkManager instance;

        public static bool mpActive;
        public static bool isServer;

        public static Queue<Action> mpActions;

        public delegate void ConnectionEstablished();
        public delegate void ConnectionFailed();

        private void Awake() {
            if (instance == null) instance = this;
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            mpActive = false;
            isServer = false;
            mpActions = new Queue<Action>();
        }

        private void Update() {
            if (mpActions.Count > 0) {
                for (int i = 0; i < mpActions.Count; i++) {
                    mpActions.Dequeue().Invoke();
                }
            }
        }

        public void InitServer() {
            // Start server
            gameObject.AddComponent<Server>();
            mpActive = true;
            isServer = true;
        }

        public void InitClient(string hostIP) {
            // Start Client
            gameObject.AddComponent<Client>().ConnectToServer(hostIP);
            mpActive = true;
            isServer = false;
        }

        public static void ListenForNetworkData(TcpClient socket) {
            NetworkStream ns = socket.GetStream();
            ns.ReadTimeout = 5000;
            byte[] buffer = new byte[1024]; // Raw byte space to receive data
            int dataSize; // Length of read bytes
            byte[] dataBytes; // Data without zeros and old data from buffer at the end

            while (socket.Connected) {
                dataSize = 0;
                try {
                    dataSize = ns.Read(buffer, 0, buffer.Length);
                    if (dataSize <= 0) break; // Connection was closed
                } catch { }

                if (dataSize > 1) { // Ignore ping byte
                    dataBytes = new byte[dataSize];
                    for (int i = 0; i < dataBytes.Length; i++) {
                        dataBytes[i] = buffer[i]; // Transfer new read bytes to dataBytes from buffer
                    }

                    // dataBytes may contain multiple commands, split them and execute each one of the them sequentially
                    foreach (NetworkCommands.NetworkCommand command in GetCommands(dataBytes)) {
                        switch ((NetworkCommands.NCType)command.type) {
                            case NetworkCommands.NCType.BEGIN_GAME:
                                mpActions.Enqueue(() => GameObject.Find("Btn Play").GetComponent<UnityEngine.UI.Button>().onClick.Invoke());
                                break;
                            case NetworkCommands.NCType.BUILD:
                                NetworkCommands.NCBuild buildCmd = (NetworkCommands.NCBuild)command;
                                mpActions.Enqueue(() => {
                                    AIGameActions.Build(GameObject.Find(buildCmd.locationName).GetComponent<GameLocation>(), (BuildingType)buildCmd.buildingTypeInt);
                                });
                                break;
                            case NetworkCommands.NCType.MOVE_TROOPS:
                                NetworkCommands.NCMoveTroops moveCmd = (NetworkCommands.NCMoveTroops)command;
                                if (moveCmd.soldierNums != null) {
                                    // Move troops with soldiers parameter
                                    Soldiers soldiers = NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(moveCmd.soldierNums);

                                    mpActions.Enqueue(() => {
                                        GameLocation origin = GameObject.Find(moveCmd.originLocationName).GetComponent<GameLocation>();
                                        GameLocation destination = GameObject.Find(moveCmd.destLocationName).GetComponent<GameLocation>();
                                        AIGameActions.MoveTroops(origin, destination, soldiers);
                                    });
                                } else {
                                    // Move troops without soldiers parameter
                                    mpActions.Enqueue(() => {
                                        GameLocation origin = GameObject.Find(moveCmd.originLocationName).GetComponent<GameLocation>();
                                        GameLocation destination = GameObject.Find(moveCmd.destLocationName).GetComponent<GameLocation>();
                                        AIGameActions.MoveTroops(origin, destination);
                                    });
                                }
                                break;
                            case NetworkCommands.NCType.RECRUIT:
                                NetworkCommands.NCRecruit recruitCmd = (NetworkCommands.NCRecruit)command;
                                mpActions.Enqueue(() => {
                                    AIGameActions.Recruit(GameObject.Find(recruitCmd.locationName).GetComponent<GameLocation>(), NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(recruitCmd.soldierNums));
                                });
                                break;

                            default:
                                Debug.LogWarning("NetworkCommand <" + command.type + "> not found");
                                break;
                        }
                    }

                }
            }
        }

        /// <summary>If receiver is specified only to him the message will be sent.
        /// If receiver is null or not given (optional) a Server call broadcasts to all clients and a Client call sends to his server.</summary>
        public static void Send(NetworkCommands.NetworkCommand command, TcpClient receiver = null) {
            if (receiver != null) {
                // To specified receiver
                try {
                    NetworkStream ns = receiver.GetStream();

                    string jsonData = command.ToSendableString();
                    byte[] sendData = EncodeSendData(jsonData);

                    ns.Write(sendData, 0, sendData.Length);
                } catch (Exception e) {
                    Debug.LogError("Sending to receiver failed: " + e);
                }
            } else {
            if (isServer) {
                // Server to client(s)
                try {
                    foreach (TcpClient client in Server.instance.clients) {
                        NetworkStream ns = client.GetStream();

                        string jsonData = command.ToSendableString();
                        byte[] sendData = EncodeSendData(jsonData);

                        ns.Write(sendData, 0, sendData.Length);
                    }
                } catch (Exception e) {
                    Debug.LogError("Sending as server failed: " + e);
                }
            } else {
                // Client to server
                try {
                    NetworkStream ns = Client.instance.server.GetStream();

                    string jsonData = command.ToSendableString();
                    byte[] sendData = EncodeSendData(jsonData);

                    ns.Write(sendData, 0, sendData.Length);
                } catch (Exception e) {
                    Debug.LogError("Sending as client failed: " + e);
                }
            }
        }
        }

        private static Stack<NetworkCommands.NetworkCommand> GetCommands(byte[] data) {
            string dataString = DecodeSendData(data);
            Stack<NetworkCommands.NetworkCommand> res = new Stack<NetworkCommands.NetworkCommand>();

            while (true) {
                string commandString = FindNextCommand(dataString);
                if (commandString.Length > 0) {
                    res.Push(NetworkCommands.NetworkCommand.CreateFromSentString(commandString));
                    dataString = dataString.Substring(commandString.Length);
                } else break;
            }

            return res;
        }

        private static string FindNextCommand(string s) {
            int endIndex = s.IndexOf('}');
            return s.Substring(0, endIndex + 1);
        }

        private static byte[] EncodeSendData(string data) {
            return Encoding.UTF8.GetBytes(data);
        }

        private static string DecodeSendData(byte[] data) {
            return Encoding.UTF8.GetString(data);
        }

        public static string GetLocalIP() {
            string localIP = "ERROR";

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }
    }
}