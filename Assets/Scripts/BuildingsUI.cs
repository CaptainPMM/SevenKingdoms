using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingsUI : MonoBehaviour {
    private InputController inputController;

    public void Init(InputController ic) {
        inputController = ic;

        // Fill the texts with the buildings infos
        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>();
        BuildingType[] buildingTypes = Building.CreateBuildingTypesArray();
        int counter = 0;
        foreach (TextMeshProUGUI txt in txts) {
            switch (txt.name) {
                case "Text Building Name":
                    txt.text = Building.GetBuildingTypeInfos(buildingTypes[counter]).buildingName;
                    break;
                case "Text Building Desc":
                    txt.text = Building.GetBuildingTypeInfos(buildingTypes[counter++]).description;
                    break;
            }
        }
    }

    void OnEnable() {
        // Get built buildings in the selected game location
        Building[] builtBuildings = inputController.selectedLocation.GetComponent<GameLocation>().buildings.ToArray();

        // Set buttons accordingly
        // TODO
    }
}
