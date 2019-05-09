using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour {
    public GameObject troopsPrefab;

    public GameObject player;
    public GameObject selectionMarker;
    public GameObject selectionUI;

    public GameObject selectedLocation;

    // Start is called before the first frame update
    void Start() {
        selectedLocation = null;
        selectionUI.GetComponent<SelectionUI>().Init(this);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                switch (hit.collider.tag) {

                    case "game_location":
                        if (selectedLocation == null) {
                            selectedLocation = hit.collider.gameObject;
                            House selHouse = selectedLocation.GetComponent<GameLocation>().house;
                            if (selHouse.houseType == player.GetComponent<GamePlayer>().house.houseType) {
                                selectionMarker.transform.position = selectedLocation.transform.position;
                                selectionMarker.GetComponentInChildren<SpriteRenderer>().color = selHouse.color;
                                selectionMarker.SetActive(true);
                                selectionUI.SetActive(true);
                            } else {
                                selectedLocation = null;
                            }
                        } else {
                            if (!hit.collider.gameObject.Equals(selectedLocation)) {
                                foreach (GameLocation l in selectedLocation.GetComponent<GameLocation>().reachableLocations) {
                                    if (l.gameObject == hit.collider.gameObject) {
                                        MoveTroops(hit.collider.gameObject);
                                        break;
                                    }
                                }
                            }

                            selectedLocation = null;
                            selectionMarker.SetActive(false);
                            selectionUI.SetActive(false);
                        }
                        break;
                    case "fighting_house":
                        if (selectedLocation != null) {
                            foreach (GameLocation l in selectedLocation.GetComponent<GameLocation>().reachableLocations) {
                                GameObject destination = hit.collider.gameObject.GetComponent<FightingHouse>().combat.location.gameObject;
                                if (l.gameObject == destination) {
                                    MoveTroops(destination);
                                    break;
                                }
                            }
                            selectedLocation = null;
                            selectionMarker.SetActive(false);
                            selectionUI.SetActive(false);
                        }
                        break;

                    default:
                        if (selectedLocation != null) {
                            selectedLocation = null;
                            selectionMarker.SetActive(false);
                            selectionUI.SetActive(false);
                        }
                        break;

                }
            }
        }
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
}