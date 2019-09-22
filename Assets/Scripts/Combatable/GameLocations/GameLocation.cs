using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLocation : Combatable {
    public static List<GameLocation> allGameLocations = new List<GameLocation>();

    public GameObject connectionLinePrefab;

    public string locationName;
    public GameLocation[] reachableLocations;
    public List<BuildingType> buildableBuildings = new List<BuildingType>();
    public List<GameEffect> locationEffects = new List<GameEffect>();
    public List<Building> buildings = new List<Building>();

    private bool wasOccupied = false;
    private float elapsedTimeResources = 0f;

    private Soldiers soldiersInRecruitment;
    protected float recruitmentSpeed;
    private float recruitmentSpeedBuffs = 1f;
    private float elapsedTimeRecruitment = 0f;
    private int recruitmentCurrSoldierTypeInt = 0;

    private float elapsedTimeGUI = 0f;
    private static GamePlayer player = null;
    private Button btnFastBuildLocalAdmin;
    private TextMeshProUGUI locatioNameText;

    public GameObject recruitmentIndicatorGO;
    public GameObject fortificationLevelGO;

    protected int BASE_GOLD_INCOME;
    protected int BASE_MANPOWER_INCOME;

    protected void Awake() {
        allGameLocations.Add(this);
        locationName = name.Substring(name.IndexOf(' ') + 1); // Filter the location type e.g Outpost Northern Reach -> Northern Reach
        soldiersInRecruitment = new Soldiers();
        recruitmentIndicatorGO.GetComponent<Image>().enabled = false;
        foreach (Button b in GetComponentsInChildren<Button>()) {
            if (b.name == "Button Build Local Admin") btnFastBuildLocalAdmin = b;
        }
    }

    // Start is called before the first frame update
    new protected void Start() {
        base.Start();

        if (player == null) player = GameController.activeGameController.player.GetComponent<GamePlayer>();
        if (player.house.houseType == this.house.houseType) GameController.activeGameController.locationsHeldByPlayer++;
        btnFastBuildLocalAdmin.gameObject.SetActive(false);

        // Setup UI panel
        foreach (TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>()) {
            if (t.name == "Text Location Name") {
                locatioNameText = t;
                t.text = locationName;

                Image panel = t.transform.parent.gameObject.GetComponent<Image>();
                Color panelColor = panel.color;
                panelColor.r = house.color.r;
                panelColor.g = house.color.g;
                panelColor.b = house.color.b;
                panel.color = panelColor; // Also change panel color to house colors
                break;
            }
        }
        DetermineFortificationLevel();

        // Create GUI lines to reachable locations
        foreach (GameLocation location in reachableLocations) {
            // Check if line is duplicate
            bool lineExists = false;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("location_conn_line")) {
                LineRenderer lr = go.GetComponent<LineRenderer>();
                if (lr.GetPosition(0) == location.transform.position && lr.GetPosition(3) == transform.position) {
                    lineExists = true;
                    break;
                }
            }

            if (!lineExists) {
                GameObject cl = Instantiate(connectionLinePrefab);
                cl.name = "LocConnLine: " + name + "-" + location.name;
                LineRenderer lr = cl.GetComponent<LineRenderer>();

                lr.SetPosition(0, transform.position); // origin
                lr.SetPosition(3, location.transform.position); // destination

                // Add middle points to support a specific gradient
                float opacity = 0.6f;
                float gradientStart = 0.4f;
                float gradientEnd = 0.6f;

                Vector3 directionVector = location.transform.position - transform.position; // destination - origin
                lr.SetPosition(1, transform.position + (directionVector * gradientStart)); // gradient start
                lr.SetPosition(2, transform.position + (directionVector * gradientEnd)); // gradient end

                Gradient g = new Gradient();
                GradientColorKey[] g_colors = {
                    new GradientColorKey(house.color, gradientStart),
                    new GradientColorKey(location.house.color, gradientEnd)
                };
                GradientAlphaKey[] g_alphas = {
                    new GradientAlphaKey(opacity, gradientStart),
                    new GradientAlphaKey(opacity, gradientEnd)
                };

                g.SetKeys(g_colors, g_alphas);
                lr.colorGradient = g;
            }
        }
    }

    protected void Update() {
        if (combat == null) {
            if (wasOccupied) {
                Start();
                wasOccupied = false;
            }

            elapsedTimeResources += Time.deltaTime;
            if (elapsedTimeResources >= Global.GAME_LOCATION_RESOURCES_UPDATE_TIME) {
                elapsedTimeResources = 0f;
                ResourcesIncomeForHouse();
            }

            if (soldiersInRecruitment.GetNumSoldiersInTotal() > 0) {
                elapsedTimeRecruitment += Time.deltaTime;
                if (elapsedTimeRecruitment >= recruitmentSpeed * recruitmentSpeedBuffs) {
                    elapsedTimeRecruitment = 0f;
                    Recruit();
                }
            }

            elapsedTimeGUI += Time.deltaTime;
            if (elapsedTimeGUI >= Global.GAME_LOCATION_GUI_UPDATE_TIME) {
                UpdateGUI();
                locatioNameText.color = Color.white;
                if (house.houseType == player.house.houseType) {
                    if (!IsLocalAdminBuilt()) {
                        locatioNameText.color = Color.yellow;
                        if (house.gold >= Building.GetBuildingTypeInfos(BuildingType.LOCAL_ADMINISTRATION).neededGold) {
                            if (!btnFastBuildLocalAdmin.gameObject.activeSelf) {
                                btnFastBuildLocalAdmin.gameObject.SetActive(true);
                            }
                        } else {
                            if (btnFastBuildLocalAdmin.gameObject.activeSelf) btnFastBuildLocalAdmin.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void OccupyBy(House house, bool afterSave = false) {
        // Safety for savegame loading
        if (player == null) player = GameController.activeGameController.player.GetComponent<GamePlayer>();

        // Let AIs and game controller know of the change
        // Remove location from old owner
        if (player.house.houseType != this.house.houseType) {
            AIPlayer aiPlayer = GameController.activeGameController.aiPlayers.Find(aip => aip.house.houseType == this.house.houseType);
            if (aiPlayer != null) aiPlayer.ownedLocations.Remove(this);
        } else {
            GameController.activeGameController.locationsHeldByPlayer--;
        }
        // Add location to new owner (only AI)
        if (player.house.houseType != house.houseType) {
            AIPlayer aiPlayer = GameController.activeGameController.aiPlayers.Find(aip => aip.house.houseType == house.houseType);
            if (aiPlayer != null) aiPlayer.ownedLocations.Add(this);
        }

        this.house = house;
        wasOccupied = true;

        // Destroy a random building after occupation combat
        if ((!Multiplayer.NetworkManager.mpActive || Multiplayer.NetworkManager.isServer) && !afterSave && buildings.Count > 0) {
            int rand = Random.Range(0, buildings.Count);
            if (Multiplayer.NetworkManager.isServer) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCDestroyBuilding(this, buildings[rand].buildingType));
            buildings.RemoveAt(rand);
            GetEffectsFromBuildings();
        }

        // Remove all connection lines
        GameObject[] lines = GameObject.FindGameObjectsWithTag("location_conn_line");
        foreach (GameObject line in lines) {
            string[] locationNames = line.name.Substring("LocConnLine: ".Length).Split('-'); // "LocConnLine: Outpost XY-Castle AB"

            if (this.name == locationNames[0] || this.name == locationNames[1]) {
                Destroy(line);
            }
        }
    }

    public void GetEffectsFromBuildings() {
        locationEffects.Clear();
        foreach (Building b in buildings) {
            locationEffects.AddRange(b.gameEffects);
        }

        recruitmentSpeedBuffs = 1f;
        foreach (GameEffect ge in locationEffects) {
            if (ge.type == GameEffectType.RECRUITMENT_SPEED) {
                recruitmentSpeedBuffs *= ge.modifierValue;
            }
        }
    }

    public void AddBuilding(Building b, bool mpSend = true) {
        buildings.Add(b);
        GetEffectsFromBuildings();
        DetermineFortificationLevel();
        if (b.buildingType == BuildingType.LOCAL_ADMINISTRATION) btnFastBuildLocalAdmin.gameObject.SetActive(false);
        if (Multiplayer.NetworkManager.mpActive && mpSend) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCBuild(this, b.buildingType));
    }

    private void ResourcesIncomeForHouse() {
        foreach (Building b in buildings) {
            // Resource income only if Local Admin is built
            if (b.buildingType == BuildingType.LOCAL_ADMINISTRATION) {
                // Gold income
                float goldIncomeEffectsModifier = 1f;
                foreach (GameEffect ge in locationEffects) {
                    if (ge.type == GameEffectType.GOLD_INCOME) {
                        goldIncomeEffectsModifier *= ge.modifierValue;
                    }
                }
                int goldIncome = Mathf.CeilToInt((float)BASE_GOLD_INCOME * goldIncomeEffectsModifier);
                house.gold += goldIncome;

                // Manpower income
                float manpowerIncomeEffectsModifier = 1f;
                foreach (GameEffect ge in locationEffects) {
                    if (ge.type == GameEffectType.MANPOWER_INCOME) {
                        manpowerIncomeEffectsModifier *= ge.modifierValue;
                    }
                }
                int manpowerIncome = Mathf.CeilToInt((float)BASE_MANPOWER_INCOME * manpowerIncomeEffectsModifier);
                house.manpower += manpowerIncome;

                break;
            }
        }
    }

    public void AddSoldiersToRecruitment(Soldiers s, bool mpSend = true) {
        soldiersInRecruitment.AddSoldiers(s);
        recruitmentIndicatorGO.GetComponent<Image>().enabled = true;
        if (Multiplayer.NetworkManager.mpActive && mpSend) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCRecruit(this, s));
    }

    public Soldiers GetSoldiersInRecruitment() {
        return soldiersInRecruitment;
    }

    private void Recruit() {
        // Iterate through all available soldier types and recruit them in order
        // Get soldier types in recruitment
        List<SoldierType> soldierTypes = new List<SoldierType>();
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            if (soldiersInRecruitment.GetSoldierTypeNum(st) > 0) soldierTypes.Add(st);
        }

        if (soldierTypes.Count > 1) {
            // Find next recruit soldier type
            while (!soldierTypes.Contains((SoldierType)recruitmentCurrSoldierTypeInt)) {
                IncrementRecruitmentCurrSoldierTypeInt();
            }
        } else {
            recruitmentCurrSoldierTypeInt = (int)soldierTypes[0];
        }

        // Recruit the soldier type (one soldier), extract from recruitment list
        SoldierType recruitType = (SoldierType)recruitmentCurrSoldierTypeInt;
        soldiers.AddSoldierTypeNum(recruitType, 1);
        soldiersInRecruitment.SetSoldierTypeNum(recruitType, soldiersInRecruitment.GetSoldierTypeNum(recruitType) - 1);

        if (soldiersInRecruitment.GetNumSoldiersInTotal() <= 0) recruitmentIndicatorGO.GetComponent<Image>().enabled = false;

        // Switch soldier type to next one after recruitment
        IncrementRecruitmentCurrSoldierTypeInt();
    }

    private void IncrementRecruitmentCurrSoldierTypeInt() {
        recruitmentCurrSoldierTypeInt = (recruitmentCurrSoldierTypeInt + 1) % Soldiers.CreateSoldierTypesArray().Length;
    }

    public void DetermineFortificationLevel() {
        int fortLvl = 0;
        foreach (Building b in buildings) {
            if (b.buildingType == BuildingType.WOODEN_WALL) fortLvl += 1;
            if (b.buildingType == BuildingType.STONE_WALL) fortLvl += 2;
            if (b.buildingType == BuildingType.ADVANCED_WALL) fortLvl += 3;
        }

        Sprite s = Resources.Load<Sprite>("FortificationLevels/" + fortLvl);
        if (s != null) {
            fortificationLevelGO.GetComponent<Image>().sprite = s;
        } else {
            throw new System.Exception("Invalid fortification level <" + fortLvl + ">: corresponding resource image could not be found");
        }
    }

    private bool IsLocalAdminBuilt() {
        return buildings.Find(b => b.buildingType == BuildingType.LOCAL_ADMINISTRATION) == null ? false : true;
    }

    public void FastBuildLocalAdmin() {
        house.gold -= Building.GetBuildingTypeInfos(BuildingType.LOCAL_ADMINISTRATION).neededGold;
        AddBuilding(Building.CreateBuildingInstance(BuildingType.LOCAL_ADMINISTRATION));
    }
}
