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
                        ExecuteCommand(command, socket);
                    }
                }
            }
        }

        private static void ExecuteCommand(NetworkCommands.NetworkCommand command, TcpClient socket) {
            switch ((NetworkCommands.NCType)command.type) {
                case NetworkCommands.NCType.BEGIN_GAME:
                    // Only relevant for clients
                    mpActions.Enqueue(() => GameObject.Find("Btn Play").GetComponent<UnityEngine.UI.Button>().onClick.Invoke());
                    break;
                case NetworkCommands.NCType.BUILD:
                    NetworkCommands.NCBuild buildCmd = (NetworkCommands.NCBuild)command;
                    if (isServer) Send(buildCmd, ignoreClient: socket); // Broadcast to clients except the sender
                    mpActions.Enqueue(() => {
                        AIGameActions.Build(GameLocation.allGameLocations.Find(gl => gl.name == buildCmd.locationName), (BuildingType)buildCmd.buildingTypeInt, false);
                    });
                    break;
                case NetworkCommands.NCType.MOVE_TROOPS:
                    NetworkCommands.NCMoveTroops moveCmd = (NetworkCommands.NCMoveTroops)command;
                    if (isServer) Send(moveCmd, ignoreClient: socket); // Broadcast to clients except the sender
                    if (moveCmd.soldierNums != null) {
                        // Move troops with soldiers parameter
                        Soldiers soldiers = NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(moveCmd.soldierNums);

                        mpActions.Enqueue(() => {
                            GameLocation origin = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.originLocationName);
                            GameLocation destination = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.destLocationName);
                            AIGameActions.MoveTroops(origin, destination, soldiers, false);
                        });
                    } else {
                        // Move troops without soldiers parameter
                        mpActions.Enqueue(() => {
                            GameLocation origin = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.originLocationName);
                            GameLocation destination = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.destLocationName);
                            AIGameActions.MoveTroops(origin, destination, false);
                        });
                    }
                    break;
                case NetworkCommands.NCType.RECRUIT:
                    NetworkCommands.NCRecruit recruitCmd = (NetworkCommands.NCRecruit)command;
                    if (isServer) Send(recruitCmd, ignoreClient: socket); // Broadcast to clients except the sender
                    mpActions.Enqueue(() => {
                        AIGameActions.Recruit(GameLocation.allGameLocations.Find(gl => gl.name == recruitCmd.locationName), NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(recruitCmd.soldierNums), false);
                    });
                    break;
                case NetworkCommands.NCType.SELECT_HOUSE_REQUEST:
                    // Only relevant for server -> server approves house selection or not depending on the picked houses already
                    NetworkCommands.NCSelectHouseReq selHouseReqCmd = (NetworkCommands.NCSelectHouseReq)command;
                    if (Server.instance.clientHouseTypes.ContainsValue((HouseType)selHouseReqCmd.houseTypeInt)) {
                        // Not approved
                        if (selHouseReqCmd.init) {
                            // Was an initial request after connecting
                            // Suggest a free house type to the client
                            int houseTypeSuggestion = 0; // Would be the neutral house

                            for (int i = 1; i < Enum.GetValues(typeof(HouseType)).Length; i++) { // Ignore neutral house (index 0)
                                if (!Server.instance.clientHouseTypes.ContainsValue((HouseType)i)) {
                                    houseTypeSuggestion = i;
                                    break;
                                }
                            }

                            Send(new NetworkCommands.NCSelectHouseRes((HouseType)selHouseReqCmd.houseTypeInt, false, true, (HouseType)houseTypeSuggestion), socket);
                        } else {
                            Send(new NetworkCommands.NCSelectHouseRes((HouseType)selHouseReqCmd.houseTypeInt, false), socket);
                        }
                    } else {
                        // Approved
                        Send(new NetworkCommands.NCSelectHouseRes((HouseType)selHouseReqCmd.houseTypeInt, true), socket);

                        // Update selected houses list
                        Server.instance.RemoveClientHouseType(socket);
                        Server.instance.clientHouseTypes.Add(socket, (HouseType)selHouseReqCmd.houseTypeInt);
                    }
                    break;
                case NetworkCommands.NCType.SELECT_HOUSE_RESPONSE:
                    // Only relevant for clients
                    NetworkCommands.NCSelectHouseRes selHouseResCmd = (NetworkCommands.NCSelectHouseRes)command;
                    if (selHouseResCmd.houseTypeIntApproved) {
                        mpActions.Enqueue(() => HouseSelMenu.instance.UpdateSelectionUI(selHouseResCmd.houseTypeInt));
                    } else {
                        if (selHouseResCmd.init) {
                            // Was an initial response after connecting
                            // Apply server suggestion (select free house type)
                            mpActions.Enqueue(() => HouseSelMenu.instance.houseSelSlider.onValueChanged.Invoke(selHouseResCmd.houseTypeSuggestionInt));
                        } else {
                            mpActions.Enqueue(() => HouseSelMenu.instance.UpdateSelectionUIOnPickedHouse(selHouseResCmd.houseTypeInt));
                        }
                    }
                    break;
                case NetworkCommands.NCType.SYNC_COMBAT:
                    // Only relevant for clients
                    NetworkCommands.NCSyncCombat syncCombatCmd = (NetworkCommands.NCSyncCombat)command;
                    mpActions.Enqueue(() => {
                        foreach (FightingHouse fh in FightingHouse.allFightingHouses) {
                            if (fh.ID == syncCombatCmd.fightingHouseID) {
                                fh.ApplyCasualties((SoldierType)syncCombatCmd.soldierTypeInt, syncCombatCmd.damage);
                            }
                        }
                    });
                    break;

                default:
                    Debug.LogWarning("NetworkCommand <" + command.type + "> not found");
                    break;
            }
        }

        /// <summary>If receiver is specified only to him the message will be sent.
        /// If receiver is null or not given (optional) a Server call broadcasts to all clients and a Client call sends to his server.
        /// If ignoreClient is specified a broadcast ignores him.</summary>
        public static void Send(NetworkCommands.NetworkCommand command, TcpClient receiver = null, TcpClient ignoreClient = null) {
            if (receiver != null) {
                // To specified receiver
                try {
                    SendDataTo(receiver, command);
                } catch (Exception e) {
                    Debug.LogError("Sending to receiver failed: " + e);
                }
            } else {
                if (isServer) {
                    // Server to client(s)
                    try {
                        if (ignoreClient == null) {
                            // Broadcast to all clients
                            foreach (TcpClient client in Server.instance.clients) {
                                SendDataTo(client, command);
                            }
                        } else {
                            // Broadcast to all clients except the ignoreClient
                            foreach (TcpClient client in Server.instance.clients) {
                                if (client == ignoreClient) continue;
                                SendDataTo(client, command);
                            }
                        }
                    } catch (Exception e) {
                        Debug.LogError("Sending as server failed: " + e);
                    }
                } else {
                    // Client to server
                    try {
                        SendDataTo(Client.instance.server, command);
                    } catch (Exception e) {
                        Debug.LogError("Sending as client failed: " + e);
                    }
                }
            }
        }

        private static void SendDataTo(TcpClient client, NetworkCommands.NetworkCommand command) {
            NetworkStream ns = client.GetStream();

            string jsonData = command.ToSendableString();
            byte[] sendData = EncodeSendData(jsonData);

            ns.Write(sendData, 0, sendData.Length);
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

        private void OnDestroy() {
            instance = null;
            mpActive = false;
            isServer = false;
            mpActions = null;
        }
    }
}