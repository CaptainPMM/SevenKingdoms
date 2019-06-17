using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour {
    private GameController gameController;
    private GameLocation attachedGameLocation;
    private Soldiers displayedSoldiers;
    private float elapsedTime;

    private bool inRecruitState;
    private Recruitment recruitment;
    private Dictionary<SoldierType, bool[]> allowedSoldierTypes;
    private Dictionary<SoldierType, bool[]> allowedSoldierTypes_TEMPLATE;

    public void Init(GameController gc) {
        gameController = gc;
        elapsedTime = 0f;
        inRecruitState = false;

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
        attachedGameLocation = gameController.selectedLocation.GetComponent<GameLocation>();
        ResetAllowedSoldiersDict();

        // Determine available soldier types by buildings
        attachedGameLocation.buildings.ForEach(building => {
            foreach (GameEffect effect in building.gameEffects) {
                if (effect.type == GameEffectType.SOLDIER_TYPE_UNLOCK) {
                    allowedSoldierTypes[(SoldierType)effect.modifierValue] = SetNextBuildingRequirementTrue(allowedSoldierTypes[(SoldierType)effect.modifierValue]);
                }
            }
        });

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
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
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
}