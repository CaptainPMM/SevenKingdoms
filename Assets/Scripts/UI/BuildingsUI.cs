using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingsUI : MonoBehaviour {
    private GameController gameController;
    private GamePlayer player;
    private TextSizeUnifier textSizeUnifier;
    private List<GameObject> disabledGameObjects;
    private Color32 buttonColorWhenBuildable;
    private Color32 buttonColorWhenBuilt = Global.BUILD_UI_BUTTON_COLOR_WHEN_BUILT;

    private BuildingType[] buildingTypes;
    private GameLocation currLocation;
    private List<BuildingType> buildableBuildings;

    private float elapsedTime;

    public void Init(GameController gc) {
        gameController = gc;
        player = gc.player.GetComponent<GamePlayer>();
        textSizeUnifier = GetComponent<TextSizeUnifier>();
        disabledGameObjects = new List<GameObject>();
        buttonColorWhenBuildable = GetComponentInChildren<Button>().GetComponent<Image>().color;
        buildingTypes = Building.CreateBuildingTypesArray();
        elapsedTime = 0f;
    }

    void OnEnable() {
        ScrollRect scroll = GetComponentInChildren<ScrollRect>();
        scroll.verticalNormalizedPosition = 1; // Goto top scroll position

        currLocation = gameController.selectedLocation.GetComponent<GameLocation>();

        // Find all buildable buildings for the current location
        buildableBuildings = new List<BuildingType>();
        foreach (BuildingType bt in buildingTypes) {
            if (IsBuildingTypeAvailable(currLocation, bt)) {
                buildableBuildings.Add(bt);
            }
        }

        // Determine if scrolling is needed
        float totalHeightNeeded = buildableBuildings.Count * scroll.content.GetChild(0).GetComponent<RectTransform>().rect.height;
        if (totalHeightNeeded <= GetComponentInChildren<Image>().GetComponent<RectTransform>().rect.height - 130f) { // Second is panel minus padding
            scroll.enabled = false;
        } else {
            scroll.enabled = true;
        }

        Setup();
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= Global.BUILD_UI_REFRESH_TIME) {
            elapsedTime = 0f;
            Setup();
        }
    }

    private void Setup() {
        List<Building> builtBuildings = currLocation.buildings;

        // Re-enable previously disabled ui elements
        foreach (GameObject go in disabledGameObjects) {
            go.SetActive(true);
        }
        disabledGameObjects.Clear();

        // Fill the texts with the buildings infos (only buildings that are allowed for the location and house)
        // Also handle buttons
        // Also clean up non available building ui entries (disable)
        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>();
        Button[] btns = GetComponentsInChildren<Button>();
        int buildingsCounter = 0; // Increases when every ui element for a building type is set (used to access building infos)
        int buildingTxtsCounter = 0; // Increases with every ui element touched, then reduced to zero again (used to increase buildingsCounter)
        foreach (TextMeshProUGUI txt in txts) {
            if (buildingsCounter >= buildableBuildings.Count) {
                // Disable not needed tailing ui elements
                if (txt.name != "Text Close Btn") {
                    disabledGameObjects.Add(txt.transform.parent.gameObject);
                    txt.transform.parent.gameObject.SetActive(false);
                }
                continue;
            }

            switch (txt.name) {
                case "Text Building Name":
                    txt.text = Building.GetBuildingTypeInfos(buildableBuildings[buildingsCounter]).buildingName;
                    buildingTxtsCounter++;
                    break;
                case "Text Building Desc":
                    txt.text = Building.GetBuildingTypeInfos(buildableBuildings[buildingsCounter]).description;
                    buildingTxtsCounter++;
                    break;
                case "Text Building Costs":
                    int neededGold = Building.GetBuildingTypeInfos(buildableBuildings[buildingsCounter]).neededGold;
                    txt.text = neededGold.ToString() + " Gold";
                    if (player.house.gold < neededGold && builtBuildings.Find(b => b.buildingType == buildableBuildings[buildingsCounter]) == null) {
                        txt.color = new Color32(240, 20, 20, 255);
                    } else {
                        txt.color = Color.white;
                    }
                    buildingTxtsCounter++;
                    break;
                case "Text Build Btn":
                    GameObject btnGO = txt.transform.parent.gameObject;
                    Button btn = btnGO.GetComponent<Button>();

                    if (builtBuildings.Find(b => b.buildingType == buildableBuildings[buildingsCounter]) != null) {
                        // Building is built in game location
                        SetButtonBuilt(btn);
                    } else {
                        // Building is not built
                        SetButtonBuildable(btn, currLocation, buildableBuildings[buildingsCounter]);

                        if (player.house.gold < Building.GetBuildingTypeInfos(buildableBuildings[buildingsCounter]).neededGold) {
                            btn.interactable = false;
                        }
                    }

                    buildingTxtsCounter++;
                    break;
            }

            if (buildingTxtsCounter >= 4) { // This magic number represents the amount of ui elements touched (number of switch entries)
                buildingTxtsCounter = 0;
                buildingsCounter++;
            }
        }

        if (textSizeUnifier != null) {
            textSizeUnifier.UnifyTextSizes();
        }
    }

    /**
        Checks if the specified building type is available to the current game location and house
     */
    private bool IsBuildingTypeAvailable(GameLocation currLoc, BuildingType bt) {
        if (currLoc.buildableBuildings.Contains(bt) && currLoc.house.buildableBuildings.Contains(bt)) {
            return true;
        } else {
            return false;
        }
    }

    private void SetButtonBuilt(Button btn) {
        btn.interactable = false;
        btn.GetComponent<Image>().color = buttonColorWhenBuilt;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Built";
    }

    private void SetButtonBuildable(Button btn, GameLocation gl, BuildingType bt) {
        btn.interactable = true;
        btn.GetComponent<Image>().color = buttonColorWhenBuildable;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Build";
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            player.house.gold -= Building.GetBuildingTypeInfos(bt).neededGold;
            gl.AddBuilding(Building.CreateBuildingInstance(bt));
            Setup();
        });
    }
}
