using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingsUI : MonoBehaviour {
    private InputController inputController;
    private List<GameObject> disabledGameObjects;
    private Color32 buttonColorWhenBuildable;
    private Color32 buttonColorWhenBuilt = Global.BUILD_UI_BUTTON_COLOR_WHEN_BUILDABLE;

    public void Init(InputController ic) {
        inputController = ic;
        disabledGameObjects = new List<GameObject>();
        buttonColorWhenBuildable = GetComponentInChildren<Button>().GetComponent<Image>().color;
    }

    void OnEnable() {
        GameLocation currLocation = inputController.selectedLocation.GetComponent<GameLocation>();
        BuildingType[] buildingTypes = Building.CreateBuildingTypesArray();
        List<Building> builtBuildings = currLocation.buildings;

        // Find all buildable buildings for the current location
        List<BuildingType> buildableBuildings = new List<BuildingType>();
        foreach (BuildingType bt in buildingTypes) {
            if (IsBuildingTypeAvailable(currLocation, bt)) {
                buildableBuildings.Add(bt);
            }
        }

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
                    txt.text = Building.GetBuildingTypeInfos(buildableBuildings[buildingsCounter]).neededGold.ToString() + " Gold";
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
        btn.GetComponent<Image>().color = buttonColorWhenBuilt;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Built";
    }

    private void SetButtonBuildable(Button btn, GameLocation gl, BuildingType bt) {
        btn.interactable = true;
        btn.GetComponent<Image>().color = buttonColorWhenBuildable;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Build";
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            gl.buildings.Add(Building.CreateBuildingInstance(bt));
            SetButtonBuilt(btn);
        });
    }
}
