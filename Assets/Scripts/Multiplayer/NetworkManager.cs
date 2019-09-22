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
            byte[] buffer = new byte[1024 * 25]; // Raw byte space to receive data
            int dataSize; // Length of read bytes
            byte[] dataBytes = new byte[0]; // Data without zeros and old data from buffer at the end, also collects data from multiple chunks of a command
            byte[] oldBytes = new byte[0]; // Temp storage for old data bytes for chunked commands/data

            while (socket.Connected) {
                dataSize = 0;
                try {
                    dataSize = ns.Read(buffer, 0, buffer.Length);
                    if (dataSize <= 0) break; // Connection was closed
                } catch { }

                if (dataSize > 1) { // Ignore ping byte
                    // If dataBytes contains old chunks...
                    if (dataBytes.Length > 0) {
                        // ...copy old bytes from data bytes array for temp storage
                        oldBytes = new byte[dataBytes.Length];
                        for (int i = 0; i < oldBytes.Length; i++) {
                            oldBytes[i] = dataBytes[i];
                        }
                    }

                    // Resize data bytes array
                    dataBytes = new byte[oldBytes.Length + dataSize];

                    // Copy old bytes into new resized data bytes array if any
                    for (int i = 0; i < oldBytes.Length; i++) {
                        dataBytes[i] = oldBytes[i];
                    }

                    // Copy new bytes from buffer
                    for (int i = 0; i < dataSize; i++) {
                        dataBytes[oldBytes.Length + i] = buffer[i];
                    }

                    // Check if command end reached or chunks are missing
                    if (buffer[dataSize - 1] != '}') {
                        continue;
                    }

                    // dataBytes may contain multiple commands, split them and execute each one of the them sequentially
                    foreach (NetworkCommands.NetworkCommand command in GetCommands(dataBytes)) {
                        if (command != null) ExecuteCommand(command, socket);
                    }

                    dataBytes = new byte[0];
                    oldBytes = new byte[0];
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
                        Soldiers moveSoldiers = NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(moveCmd.soldierNums);

                        mpActions.Enqueue(() => {
                            GameLocation origin = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.originLocationName);
                            GameLocation destination = GameLocation.allGameLocations.Find(gl => gl.name == moveCmd.destLocationName);
                            AIGameActions.MoveTroops(origin, destination, moveSoldiers, false);
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

                    Soldiers recruitSoldiers = NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(recruitCmd.soldierNums);
                    mpActions.Enqueue(() => {
                        AIGameActions.Recruit(GameLocation.allGameLocations.Find(gl => gl.name == recruitCmd.locationName), recruitSoldiers, false);
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

                    FightingHouse foundFh = FindFightingHouseByID(syncCombatCmd.fightingHouseID, syncCombatCmd.fhFallbackID);

                    if (foundFh != null) foundFh.ApplyCasualties((SoldierType)syncCombatCmd.soldierTypeInt, syncCombatCmd.damage);
                    else {
                        Debug.LogError("NetworkManager Error: SYNC_COMBAT FightingHouse <" + syncCombatCmd.fightingHouseID + "> not found");
                        NetworkManager.Send(new NetworkCommands.NCSyncGame(true));
                    }
                    break;
                case NetworkCommands.NCType.SYNC_COMBAT_END:
                    // Only relevant for clients
                    NetworkCommands.NCSyncCombatEnd syncCombatEndCmd = (NetworkCommands.NCSyncCombatEnd)command;

                    FightingHouse winnerFightingHouse = FindFightingHouseByID(syncCombatEndCmd.winnerFightingHouseID, syncCombatEndCmd.winnerFhFallbackID);

                    mpActions.Enqueue(() => {
                        if (winnerFightingHouse != null) {
                            // Sync soldiers of the combat participants
                            // ATTENTION! The winner gets comnpletly new soldiers with refreshed HP. Mostly this will only be the first soldier of a type.
                            // But if soldier types with very high HP are introduced in the future this can cause invincible soldiers!
                            // A fix would be to send the HPs for each soldier, but this bloats the network traffic quite a bit, so currently this little "feature" is O.K.
                            // ==> ACTUALLY the calculation of damage etc. is on server where this "feauture" is not present, so everything should be fine.
                            // Here only the UI presentation is important not the soldier stats. The only thing that may be off is the casualties popup (only by small numbners).

                            // Set winner soldiers
                            int i = 0;
                            foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                                winnerFightingHouse.soldiers.SetSoldierTypeNum(st, syncCombatEndCmd.winnerRemainingSoldierNums[i++]);
                            }
                            // Remove loosers soldiers
                            foreach (FightingHouse fh in winnerFightingHouse.combat.fightingHouses) {
                                if (fh != winnerFightingHouse) fh.soldiers.RemoveSoldiers(fh.soldiers); // All loosers have 0 soldiers now
                            }

                            winnerFightingHouse.combat.DetermineCombatStatus();
                        } else {
                            Debug.LogError("NetworkManager Error: SYNC_COMBAT_END FightingHouse <" + syncCombatEndCmd.winnerFightingHouseID + "> not found");
                            NetworkManager.Send(new NetworkCommands.NCSyncGame(true));
                        }
                    });
                    break;
                case NetworkCommands.NCType.DESTROY_BUILDING:
                    // Only relevant for clients
                    NetworkCommands.NCDestroyBuilding remBuildingCmd = (NetworkCommands.NCDestroyBuilding)command;
                    mpActions.Enqueue(() => {
                        GameLocation remLocation = GameLocation.allGameLocations.Find(gl => gl.name == remBuildingCmd.locationName);
                        Building remBuilding = null;
                        foreach (Building b in remLocation.buildings) {
                            if (b.buildingType == (BuildingType)remBuildingCmd.buildingTypeInt) remBuilding = b;
                        }

                        if (remBuilding != null) {
                            remLocation.buildings.Remove(remBuilding);
                            remLocation.GetEffectsFromBuildings();
                        } else {
                            Debug.LogError("NetworkManager Error: DESTROY_BUILDING buildingTypeInt <" + remBuildingCmd.buildingTypeInt + "> not found");
                            NetworkManager.Send(new NetworkCommands.NCSyncGame(true));
                        }
                    });
                    break;
                case NetworkCommands.NCType.SYNC_GAME:
                    NetworkCommands.NCSyncGame syncGameCmd = (NetworkCommands.NCSyncGame)command;

                    if (syncGameCmd.isRequest) {
                        // Only relevant for server
                        // Show UI info for host player
                        mpActions.Enqueue(() => ConnInfoPanel.instance.ShowPanel("Game desync detected"));
                        Debug.LogWarning("Game desync detected");
                    } else {
                        // Only relevant for clients
                        // Load save game
                        mpActions.Clear();
                        mpActions.Enqueue(() => GameController.activeGameController.HandleFastSaveGameData(syncGameCmd.saveGameData));
                    }
                    break;

                default:
                    Debug.LogWarning("NetworkCommand <" + command.type + "> not found");
                    NetworkManager.Send(new NetworkCommands.NCSyncGame(true));
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

        private static FightingHouse FindFightingHouseByID(string ID, string fallbackID) {
            foreach (FightingHouse fh in FightingHouse.allFightingHouses) {
                if (fh.ID == ID) {
                    return fh;
                }
            }

            // Fallback fighting house id search
            foreach (FightingHouse fh in FightingHouse.allFightingHouses) {
                if (fh.fallbackID == fallbackID) {
                    return fh;
                }
            }

            // No luck today :(
            return null;
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