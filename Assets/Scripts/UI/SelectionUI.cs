using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour {
    private GameController gameController;
    private Combatable attachedGameLocation; // GameLocation or FightingHouse
    private Soldiers displayedSoldiers;
    private float elapsedTime;

    private bool inRecruitState;

    private GameObject infoPanel;
    private GameObject costsPanel;
    private TextMeshProUGUI totalGoldCosts;
    private TextMeshProUGUI totalMpCosts;

    private Recruitment recruitment;

    private Dictionary<SoldierType, bool[]> allowedSoldierTypes;
    private Dictionary<SoldierType, bool[]> allowedSoldierTypes_TEMPLATE;

    public void Init(GameController gc) {
        gameController = gc;
        elapsedTime = 0f;
        inRecruitState = false;

        // Set recruit info gold costs
        int counter = 0; // Count soldier types
        foreach (TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>()) {
            if (t.name.StartsWith("Text Gold Costs")) {
                t.text = Soldiers.GetSoldierTypeStats((SoldierType)counter).goldCosts.ToString();
                counter++;
            } else if (t.name.StartsWith("Text Gold Sum Costs")) {
                totalGoldCosts = t;
            } else if (t.name.StartsWith("Text MP Sum Costs")) {
                totalMpCosts = t;
            }
        }

        // Disable recruit info panels
        Image[] imgs = GetComponentsInChildren<Image>();
        Image infoPanelImg = System.Array.Find(imgs, i => i.name == "Info Panel");
        Image costsPanelImg = System.Array.Find(imgs, i => i.name == "Costs Panel");

        if (infoPanelImg == null || costsPanelImg == null) {
            Debug.LogError("Selection UI: could not find Info Panel<" + infoPanelImg + "> or Costs Panel<" + costsPanelImg + ">");
        } else {
            infoPanel = infoPanelImg.gameObject;
            costsPanel = costsPanelImg.gameObject;
            ToggleInfoPanels();
        }

        // Fill allowed soldier types dictionary with template (every bool value represents a building that is needed)
        // If the last bool array item is true -> every requirement is met -> soldier type recruitable
        // Set the template with all values = false as a reset object
        allowedSoldierTypes_TEMPLATE = new Dictionary<SoldierType, bool[]>();
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            int requiredBuildings = 0;
            foreach (BuildingType bt in System.Enum.GetValues(typeof(BuildingType))) {
                GameEffect[] effects = Building.GetBuildingTypeInfos(bt).gameEffects;
                foreach (GameEffect e in effects) {
                    if (e.type == GameEffectType.SOLDIER_TYPE_UNLOCK && e.modifierValue == (int)st) {
                        requiredBuildings++;
                    }
                }
            }
            allowedSoldierTypes_TEMPLATE.Add(st, new bool[requiredBuildings]); // init value is false
        }
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= Global.SEL_UI_UPDATE_TIME) {
            elapsedTime = 0f;
            if (inRecruitState) {
                SetupRecruitSliders();
            } else {
                if (displayedSoldiers.GetNumSoldiersInTotal() != attachedGameLocation.soldiers.GetNumSoldiersInTotal()) {
                    UpdateSoldierSelectionSliders();
                }
            }
        }
    }

    void OnEnable() {
        attachedGameLocation = gameController.selectedLocation.GetComponent<Combatable>();
        // Check if maybe not a game location is selected
        if (gameController.selectedLocation.GetComponent<GameLocation>() != null) {
            ResetAllowedSoldiersDict();

            // Determine available soldier types by buildings
            ((GameLocation)attachedGameLocation).buildings.ForEach(building => {
                foreach (GameEffect effect in building.gameEffects) {
                    if (effect.type == GameEffectType.SOLDIER_TYPE_UNLOCK) {
                        allowedSoldierTypes[(SoldierType)effect.modifierValue] = SetNextBuildingRequirementTrue(allowedSoldierTypes[(SoldierType)effect.modifierValue]);
                    }
                }
            });
        }

        DefaultState();
    }

    /**
        Reset all values to false in the correct structure
     */
    private void ResetAllowedSoldiersDict() {
        allowedSoldierTypes = new Dictionary<SoldierType, bool[]>();
        foreach (KeyValuePair<SoldierType, bool[]> kv in allowedSoldierTypes_TEMPLATE) {
            bool[] val = new bool[kv.Value.Length];
            for (int i = 0; i < val.Length; i++) {
                val[i] = kv.Value[i];
            }
            allowedSoldierTypes.Add(kv.Key, val);
        }
    }

    private bool[] SetNextBuildingRequirementTrue(bool[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == false) {
                arr[i] = true;
                break;
            }
        }
        return arr;
    }

    private void DefaultState() {
        inRecruitState = false;
        ToggleInfoPanels();

        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            s.onValueChanged.RemoveAllListeners();
        }

        UpdateSoldierSelectionSliders();

        Button[] btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            if (btn.name == "Button Recruit") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_recruit");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    RecruitState();
                });
            } else if (btn.name == "Button Build") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_build");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    ClickedBuildBtn();
                });
            }
        }
    }

    private void UpdateSoldierSelectionSliders() {
        displayedSoldiers = new Soldiers();
        displayedSoldiers.AddSoldiers(attachedGameLocation.soldiers);

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>(true)) {
            s.maxValue = displayedSoldiers.GetSoldierTypeNum((SoldierType)counter);
            s.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = s.value.ToString();
            if (s.maxValue <= 0) {
                s.transform.Find("Fill Area").gameObject.SetActive(false);
            } else {
                s.transform.Find("Fill Area").gameObject.SetActive(true);
                s.value = s.maxValue;
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void ClickedBuildBtn() {
        gameController.OpenBuildingsMenu();
    }

    private void RecruitState() {
        inRecruitState = true;
        ToggleInfoPanels();
        totalGoldCosts.text = "0";
        totalMpCosts.text = "0";

        Button[] btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            if (btn.name == "Button Recruit") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_back");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    DefaultState();
                });
            } else if (btn.name == "Button Build") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_recruit");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    ClickedFinalRecruitBtn();
                });
            }
        }

        recruitment = new Recruitment(gameController.player.GetComponent<GamePlayer>());
        SetupRecruitSliders();
    }

    private void SetupRecruitSliders() {
        recruitment.Update();
        totalGoldCosts.text = recruitment.GetGoldCosts().ToString();
        totalMpCosts.text = recruitment.GetMpCosts().ToString();

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            // Check if the current soldier type is allowed, last array item has to be true
            SoldierType st = (SoldierType)counter;
            if (allowedSoldierTypes[st][allowedSoldierTypes[st].Length - 1] == true) {
                int currSoldierTypeNum = recruitment.GetRecruitSoldiers().GetSoldierTypeNum(st);
                s.value = currSoldierTypeNum;
                s.maxValue = currSoldierTypeNum + recruitment.GetMaxAvailableSoldierTypeNum(st);

                s.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = s.value + " / " + s.maxValue;

                if (s.value <= 0 && s.maxValue <= 0) {
                    s.transform.Find("Fill Area").gameObject.SetActive(false);
                } else {
                    s.transform.Find("Fill Area").gameObject.SetActive(true);
                    s.onValueChanged.RemoveAllListeners();
                    s.onValueChanged.AddListener(val => {
                        recruitment.SetRecruitSoldierTypeNum(st, (int)val);
                        SetupRecruitSliders();
                    });
                }
            } else {
                // Not allowed, not all building requirements are met
                s.value = 0;
                s.transform.Find("Fill Area").gameObject.SetActive(false);
                s.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Buildings missing";
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void ClickedFinalRecruitBtn() {
        GamePlayer player = gameController.player.GetComponent<GamePlayer>();
        player.house.gold -= recruitment.GetGoldCosts();
        player.house.manpower -= recruitment.GetMpCosts();

        attachedGameLocation.soldiers.AddSoldiers(recruitment.GetRecruitSoldiers());

        DefaultState();
    }

    private void ToggleInfoPanels() {
        infoPanel.SetActive(inRecruitState);
        costsPanel.SetActive(inRecruitState);
    }

    public void EnableOnlyInfoMode() {
        ToggleInfoMode(true);
    }

    public void DisableOnlyInfoMode() {
        ToggleInfoMode(false);
    }

    private void ToggleInfoMode(bool on) {
        foreach (Button btn in GetComponentsInChildren<Button>(true)) {
            if (btn.name != "Button Close") {
                btn.gameObject.SetActive(!on);
            }
        }
        foreach (Slider s in GetComponentsInChildren<Slider>(true)) {
            s.gameObject.SetActive(!on);
        }
    }
}