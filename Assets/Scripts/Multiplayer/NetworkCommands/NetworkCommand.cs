namespace Multiplayer {
    namespace NetworkCommands {
        public abstract class NetworkCommand {
            public abstract int type { get; }

            /// <summary>Create a string that is sendable over network.
            /// The command is converted into JSON data and a header for the command type added.
            /// For example: 1:{...} where 1 is a NCType as int</summary>
            public string ToSendableString() {
                return type + ":" + ToJSON();
            }

            /// <summary>Create a NetworkCommand object from a sent string formatted by ToSendableString().</summary>
            public static NetworkCommand CreateFromSentString(string receivedString) {
                NCType type = (NCType)int.Parse(receivedString.Substring(0, 1)); // ATTENTION! This line only works as long as there are max 10 NCTypes
                string json = receivedString.Substring(2); // ATTENTION! This line only works as long as there are max 10 NCTypes
                switch (type) {
                    case NCType.BEGIN_GAME:
                        return UnityEngine.JsonUtility.FromJson<NCBeginGame>(json);
                    case NCType.BUILD:
                        return UnityEngine.JsonUtility.FromJson<NCBuild>(json);
                    case NCType.MOVE_TROOPS:
                        return UnityEngine.JsonUtility.FromJson<NCMoveTroops>(json);
                    case NCType.RECRUIT:
                        return UnityEngine.JsonUtility.FromJson<NCRecruit>(json);
                    case NCType.SELECT_HOUSE_REQUEST:
                        return UnityEngine.JsonUtility.FromJson<NCSelectHouseReq>(json);
                    case NCType.SELECT_HOUSE_RESPONSE:
                        return UnityEngine.JsonUtility.FromJson<NCSelectHouseRes>(json);
                    case NCType.SYNC_COMBAT:
                        return UnityEngine.JsonUtility.FromJson<NCSyncCombat>(json);

                    default:
                        UnityEngine.Debug.LogWarning("Could not find NCType <" + type + ">");
                        return null;
                }
            }

            /// <summary>Convert a soldiers object to a network sendable int array containing soldier type nums.</summary>
            protected static int[] SoldiersObjToNumsArray(Soldiers s) {
                System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
                foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                    list.Add(s.GetSoldierTypeNum(st));
                }
                return list.ToArray();
            }

            /// <summary>Convert a soldiers nums array of a sent command formatted by SoldiersObjToNumsArray() to a soldiers object.</summary>
            public static Soldiers SoldiersNumsArrayToObj(int[] nums) {
                Soldiers retSoldiers = new Soldiers();
                int i = 0;
                foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                    retSoldiers.SetSoldierTypeNum(st, nums[i++]);
                }
                return retSoldiers;
            }

            private string ToJSON() {
                return UnityEngine.JsonUtility.ToJson(this);
            }
        }
    }
}