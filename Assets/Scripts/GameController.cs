using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public GameObject troopsPrefab;

    public GameObject player;
    public GameObject topBarUI;
    public GameObject selectionMarker;
    public GameObject selectionUI;
    public GameObject buildingsUI;

    public GameObject selectedLocation;

    // Start is called before the first frame update
    void Start() {
        selectedLocation = null;
        topBarUI.GetComponent<TopBarUI>().Init(this);
        selectionUI.GetComponent<SelectionUI>().Init(this);
        buildingsUI.GetComponent<BuildingsUI>().Init(this);
        selectionMarker.GetComponentInChildren<SpriteRenderer>().color = player.GetComponent<GamePlayer>().house.color;
    }

    // Update is called once per frame
    void Update() {
        // ### TAPPING -> selection ###
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                switch (hit.collider.tag) {

                    case "game_location":
                        DeselectLocation();
                        GameObject targetLocation = hit.collider.gameObject;
                        if (IsLocationOwnedByPlayer(targetLocation)) {
                            // Target belongs to player house
                            SelectLocation(targetLocation);
                        } else {
                            // Show info panel for enemies game location if outside FOW
                            if (IsNeighbourOfAnyPlayerLocation(targetLocation)) {
                                SelectLocation(targetLocation);
                                selectionUI.GetComponent<SelectionUI>().EnableOnlyInfoMode();
                            }
                        }
                        break;

                    case "fighting_house":
                        // Only show info panel
                        break;

                    case "troops":
                        // Same to fighting house
                        break;

                    default:
                        if (selectedLocation != null) {
                            DeselectLocation();
                            buildingsUI.SetActive(false);
                        }
                        break;

                }
            }
        }
    }

    private bool IsLocationOwnedByPlayer(GameObject location) {
        return location.GetComponent<GameLocation>().house.houseType == player.GetComponent<GamePlayer>().house.houseType;
    }

    private void SelectLocation(GameObject targetLocation) {
        selectedLocation = targetLocation;
        selectionMarker.transform.position = selectedLocation.transform.Find("Flag").position;
        selectionMarker.SetActive(true);
        selectionUI.SetActive(true);
    }

    private void DeselectLocation() {
        selectedLocation = null;
        selectionMarker.SetActive(false);
        selectionUI.GetComponent<SelectionUI>().DisableOnlyInfoMode();
        selectionUI.SetActive(false);
    }

    // Important for move troops
    private bool IsSelLocationNeighbour(GameObject targetLocation) {
        foreach (GameLocation gl in selectedLocation.GetComponent<GameLocation>().reachableLocations) {
            if (gl.gameObject == targetLocation.gameObject) {
                return true;
            }
        }
        return false;
    }

    private bool IsNeighbourOfAnyPlayerLocation(GameObject targetLocation) {
        foreach (GameLocation gl in targetLocation.GetComponent<GameLocation>().reachableLocations) {
            if (gl.house.houseType == player.GetComponent<GamePlayer>().house.houseType) {
                return true;
            }
        }
        return false;
    }

    private void MoveTroops(GameObject toLocation) {
        GameLocation gl = selectedLocation.GetComponent<GameLocation>();
        if (gl.numSoldiers > 0) {
            Soldiers moveSoldiers = new Soldiers();
            System.Array soldierTypes = Soldiers.CreateSoldierTypesArray();
            int counter = 0;
            foreach (Slider s in selectionUI.GetComponentsInChildren<Slider>()) {
                moveSoldiers.SetSoldierType((SoldierType)counter, gl.soldiers.ExtractSoldiers((SoldierType)counter, (int)s.value));
                counter++;
            }

            if (moveSoldiers.GetNumSoldiersInTotal() > 0) {
                GameObject go = Instantiate(troopsPrefab, selectedLocation.transform.position, selectedLocation.transform.rotation);
                go.name = "Troops " + selectedLocation.name + "-" + toLocation.name;
                Troops troops = go.GetComponent<Troops>();
                troops.house = gl.house;
                troops.soldiers = moveSoldiers;
                gl.UpdateGUI();
                troops.toLocation = toLocation;
            }
        }
    }

    public void OpenBuildingsMenu() {
        selectionUI.SetActive(false);
        buildingsUI.SetActive(true);
    }
}