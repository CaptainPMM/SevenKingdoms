using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLocation : Combatable {
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

    public GameObject recruitmentIndicatorGO;
    public GameObject fortificationLevelGO;

    protected int BASE_GOLD_INCOME;
    protected int BASE_MANPOWER_INCOME;

    // Start is called before the first frame update
    new protected void Start() {
        base.Start();

        // Setup UI panel
        locationName = name.Substring(name.IndexOf(' ') + 1); // Filter the location type e.g Outpost Northern Reach -> Northern Reach
        foreach (TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>()) {
            if (t.name == "Text Location Name") {
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
        recruitmentIndicatorGO.GetComponent<Image>().enabled = false;
        DetermineFortificationLevel();

        Transform flag = gameObject.transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = house.color;
        flag.GetChild(0).Find("House Flag").GetComponent<SpriteRenderer>().sprite = house.houseFlag;

        soldiersInRecruitment = new Soldiers();

        // Create GUI lines to reachable locations
        foreach (GameLocation location in reachableLocations) {
            // Check if line is duplicate
            bool lineExists = false;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("location_conn_line")) {
                LineRenderer lr = go.GetComponent<LineRenderer>();
                if (lr.GetPosition(0) == location.transform.position && lr.GetPosition(1) == transform.position) {
                    lineExists = true;
                    break;
                }
            }

            if (!lineExists) {
                GameObject cl = Instantiate(connectionLinePrefab);
                cl.name = "LocConnLine: " + name + "-" + location.name;
                LineRenderer lr = cl.GetComponent<LineRenderer>();

                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, location.transform.position);

                Gradient g = new Gradient();
                GradientColorKey[] g_colors = {
                    new GradientColorKey(house.color, 0.45f),
                    new GradientColorKey(location.house.color, 0.55f)
                };
                GradientAlphaKey[] g_alphas = {
                    new GradientAlphaKey(0.62f, 0.45f),
                    new GradientAlphaKey(0.62f, 0.55f)
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

            UpdateGUI();
        }
    }

    public void OccupyBy(House house) {
        this.house = house;
        wasOccupied = true;

        // Destroy a random building after occupation combat
        if (buildings.Count > 0) {
            buildings.RemoveAt(Random.Range(0, buildings.Count));
            GetEffectsFromBuildings();
        }

        // Remove all connection lines
        GameObject[] lines = GameObject.FindGameObjectsWithTag("location_conn_line");
        foreach (GameObject line in lines) {
            if (line.name.Contains(name)) {
                Destroy(line);
            }
        }
    }

    protected void GetEffectsFromBuildings() {
        locationEffects.Clear();
        foreach (Building b in buildings) {
            locationEffects.AddRange(b.gameEffects);
        }

        recruitmentSpeedBuffs = 1f;
        foreach (GameEffect ge in locationEffects) {
            if (ge.type == GameEffectType.RECRUITMENT_SPEED) {
                recruitmentSpeed *= ge.modifierValue;
            }
        }
    }

    public void AddBuilding(Building b) {
        buildings.Add(b);
        GetEffectsFromBuildings();
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

    public void AddSoldiersToRecruitment(Soldiers s) {
        soldiersInRecruitment.AddSoldiers(s);
    }

    public Soldiers GetSoldiersInRecruitment() {
        return soldiersInRecruitment;
    }

    private void Recruit() {
        // Get soldier types in recruitment
        List<SoldierType> soldierTypes = new List<SoldierType>();
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            if (soldiersInRecruitment.GetSoldierTypeNum(st) > 0) soldierTypes.Add(st);
        }

        // Determine random recruitment of a soldier type in this iteration
        int rand = Random.Range(0, soldierTypes.Count);

        // Recruit the soldier type (one soldier), extract from recruitment list
        soldiers.AddSoldierTypeNum(soldierTypes[rand], 1);
        soldiersInRecruitment.SetSoldierTypeNum(soldierTypes[rand], soldiersInRecruitment.GetSoldierTypeNum(soldierTypes[rand]) - 1);

        if (soldiersInRecruitment.GetNumSoldiersInTotal() <= 0) recruitmentIndicatorGO.GetComponent<Image>().enabled = false;
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
}
