public class SaveGameLocationData {
    public string locationName;
    public int houseTypeInt;
    public int[] buildingTypeInts;
    public int[] soldiers;
    public int[] soldiersInRecruitment;

    public SaveGameLocationData(GameLocation gl) {
        locationName = gl.locationName;
        houseTypeInt = (int)gl.house.houseType;

        buildingTypeInts = new int[gl.buildings.Count];
        for (int i = 0; i < gl.buildings.Count; i++) {
            buildingTypeInts[i] = (int)gl.buildings[i].buildingType;
        }

        soldiers = Multiplayer.NetworkCommands.NetworkCommand.SoldiersObjToNumsArray(gl.soldiers);
        soldiersInRecruitment = Multiplayer.NetworkCommands.NetworkCommand.SoldiersObjToNumsArray(gl.GetSoldiersInRecruitment());
    }
}