using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingsUI : MonoBehaviour {
    private GameController gameController;
    private GamePlayer player;

    private BuildingType[] buildingTypes;
    private GameLocation currLocation;
    private List<BuildingType> viewBuildings;

    private float elapsedTime;

    public void Init(GameController gc) {
        gameController = gc;
        player = gc.player.GetComponent<GamePlayer>();
        buildingTypes = Building.CreateBuildingTypesArray();
        elapsedTime = 0f;
    }

    void OnEnable() {
        currLocation = gameController.selectedLocation.GetComponent<GameLocation>();

        // Find all buildings to show in the menu (built buildings + buildable buildings)
        viewBuildings = new List<BuildingType>();
        foreach (BuildingType bt in buildingTypes) {
            if (currLocation.buildings.Find(b => b.buildingType == bt) != null || IsBuildingTypeAvailable(currLocation, bt)) {
                viewBuildings.Add(bt);
            }
        }

        // Set scrolling height
        ScrollRect scroll = GetComponentInChildren<ScrollRect>();
        RectTransform scrollContentRect = scroll.content.GetComponent<RectTransform>();
        scrollContentRect.sizeDelta = new Vector2(scrollContentRect.sizeDelta.x, scrollContentRect.GetChild(0).GetComponent<RectTransform>().rect.height * (viewBuildings.Count + 2)); // +2 hack (padding)
        scroll.verticalNormalizedPosition = 1; // Goto top scroll position

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
        // Fill the texts with the buildings infos (only buildings that are allowed for the location and house)
        // Also handle buttons
        // Also clean up non available building ui entries (disable)
        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>(true);
        int buildingsCounter = 0; // Increases when every ui element for a building type is set (used to access building infos)
        int buildingTxtsCounter = 0; // Increases with every ui element touched, then reduced to zero again (used to increase buildingsCounter)
        foreach (TextMeshProUGUI txt in txts) {
            if (buildingsCounter >= viewBuildings.Count) {
                // Disable not needed tailing ui elements
                txt.transform.parent.gameObject.SetActive(false);
                continue;
            } else {
                txt.transform.parent.gameObject.SetActive(true);
            }

            switch (txt.name) {
                case "Text Building Name":
                    txt.text = Building.GetBuildingTypeInfos(viewBuildings[buildingsCounter]).buildingName;
                    buildingTxtsCounter++;
                    break;
                case "Text Building Desc":
                    txt.text = Building.GetBuildingTypeInfos(viewBuildings[buildingsCounter]).description;
                    buildingTxtsCounter++;
                    break;
                case "Text Building Costs":
                    int neededGold = Building.GetBuildingTypeInfos(viewBuildings[buildingsCounter]).neededGold;
                    txt.text = neededGold.ToString();
                    if (player.house.gold < neededGold && currLocation.buildings.Find(b => b.buildingType == viewBuildings[buildingsCounter]) == null) {
                        txt.color = new Color32(240, 20, 20, 255);
                    } else {
                        txt.color = Color.white;
                    }
                    buildingTxtsCounter++;
                    break;
                case "Text Build Btn (dummy)":
                    GameObject btnGO = txt.transform.parent.gameObject;
                    Button btn = btnGO.GetComponent<Button>();

                    if (currLocation.buildings.Find(b => b.buildingType == viewBuildings[buildingsCounter]) != null) {
                        // Building is built in game location
                        SetButtonBuilt(btn);
                    } else {
                        // Building is not built
                        SetButtonBuildable(btn, currLocation, viewBuildings[buildingsCounter]);

                        if (player.house.gold < Building.GetBuildingTypeInfos(viewBuildings[buildingsCounter]).neededGold) {
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
        btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_ok");
    }

    private void SetButtonBuildable(Button btn, GameLocation gl, BuildingType bt) {
        btn.interactable = true;
        btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_build");
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            player.house.gold -= Building.GetBuildingTypeInfos(bt).neededGold;
            gl.AddBuilding(Building.CreateBuildingInstance(bt));
            Setup();
        });
    }
}
