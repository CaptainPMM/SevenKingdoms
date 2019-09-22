public class SaveGameHouseData {
    public int houseTypeInt;
    public int gold;
    public int mp;

    public SaveGameHouseData(House h, AIGoldPool goldPool) {
        houseTypeInt = (int)h.houseType;
        mp = h.manpower;
        gold = goldPool.buildingGold + goldPool.recruitmentGold;
    }
}