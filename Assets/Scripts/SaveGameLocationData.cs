public class SaveGameLocationData {
    /// <summary>Location name</summary>
    public string n;
    /// <summary>House type as integer of the location</summary>
    public int h;
    /// <summary>Buildings built as building types array</summary>
    public int[] b;
    /// <summary>Soldiers of the location</summary>
    public int[] s;
    /// <summary>Soldiers in recruitment</summary>
    public int[] r;

    public SaveGameLocationData(GameLocation gl) {
        n = gl.locationName;
        h = (int)gl.house.houseType;

        b = new int[gl.buildings.Count];
        for (int i = 0; i < gl.buildings.Count; i++) {
            b[i] = (int)gl.buildings[i].buildingType;
        }

        s = Multiplayer.NetworkCommands.NetworkCommand.SoldiersObjToNumsArray(gl.soldiers);
        r = Multiplayer.NetworkCommands.NetworkCommand.SoldiersObjToNumsArray(gl.GetSoldiersInRecruitment());
    }
}