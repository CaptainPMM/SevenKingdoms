using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {
    public static GameController activeGameController;

    public GameObject troopsPrefab;

    public GameObject player;
    public GameObject topBarUI;
    public GameObject selectionMarker;
    public GameObject moveMarker;
    public GameObject moveTargetMarker;
    public GameObject selectionUI;
    public GameObject buildingsUI;
    public GameObject mpResyncBtn;

    public bool dragging = false;

    public int locationsHeldByPlayer = 0;

    public GameObject selectedLocation;

    public List<AIPlayer> aiPlayers;
    private float aiElapsedTime = 0f;
    private int aiPlayerCounter = 0;

    private Slider[] selectionSliders;
    private SpriteRenderer moveMarkerSpriteRenderer;
    private TextMeshProUGUI moveMarkerText;
    private int moveSoldiersNum;

    private void Awake() {
        activeGameController = this;
        if (Global.GAME_PARAM_PLAYER_HOUSE_TYPE != HouseType.NEUTRAL) player.GetComponent<GamePlayer>().house = new House(Global.GAME_PARAM_PLAYER_HOUSE_TYPE);
    }

    // Start is called before the first frame update
    void Start() {
        selectedLocation = null;
        topBarUI.GetComponent<TopBarUI>().Init(this);
        selectionUI.GetComponent<SelectionUI>().Init(this);
        selectionSliders = selectionUI.GetComponentsInChildren<Slider>();
        buildingsUI.GetComponent<BuildingsUI>().Init(this);
        Color playerColor = player.GetComponent<GamePlayer>().house.color;
        selectionMarker.GetComponentInChildren<SpriteRenderer>().color = playerColor;
        moveTargetMarker.GetComponentInChildren<SpriteRenderer>().color = playerColor;

        moveMarkerSpriteRenderer = moveMarker.GetComponentInChildren<SpriteRenderer>();
        playerColor.a = 210f / 255f;
        moveMarkerSpriteRenderer.color = playerColor;
        moveMarkerText = moveMarker.GetComponentInChildren<TextMeshProUGUI>();

        // Init AIs
        if (!Multiplayer.NetworkManager.mpActive || Multiplayer.NetworkManager.isServer) {
            aiPlayers = new List<AIPlayer>();
            GamePlayer p = player.GetComponent<GamePlayer>();
            // For singelplayer there are 6 ai players (seven kingdoms - 1 human player)
            // Iterate through all HouseTypes and skip the player house type and the Neutral HouseType (index 0)
            for (int i = 1; i <= 7; i++) {
                if (p.house.houseType == (HouseType)i) continue;
                if (Multiplayer.NetworkManager.mpActive && Multiplayer.Server.instance.clientHouseTypes.ContainsValue((HouseType)i)) continue;
                aiPlayers.Add(new AIPlayer((HouseType)i));
            }
        }

        // Load savegame if present
        if (Global.SAVE_GAME != null) {
            LoadSaveGame();
        }

        if (Multiplayer.NetworkManager.mpActive && Multiplayer.NetworkManager.isServer) mpResyncBtn.SetActive(true);
        else mpResyncBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        // ### CLICKING -> selection ###
        if (Input.GetMouseButtonDown(0)) {
            if (!WasClickOnUI()) { // also check if the mouse was clicked over an UI element
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    if (!hit.collider.gameObject.Equals(selectedLocation)) {
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
                                // Only show info panel if outside FOW
                                DeselectLocation();
                                GameObject target = hit.collider.gameObject;
                                bool valid = false;

                                FightingHouse fh = target.GetComponent<FightingHouse>();
                                if (fh.combat.location != null) {
                                    // Game location combat
                                    if (IsNeighbourOfAnyPlayerLocation(fh.combat.location.gameObject)) {
                                        valid = true;
                                    }
                                } else {
                                    // Combat on field
                                    Troops t = fh.firstParticipant as Troops;
                                    if (IsNeighbourOfAnyPlayerLocation(t.toLocation) || IsLocationOwnedByPlayer(t.toLocation)) {
                                        valid = true;
                                    }
                                }

                                if (valid) {
                                    SelectLocation(target);
                                    selectionUI.GetComponent<SelectionUI>().EnableOnlyInfoMode();
                                }
                                break;

                            default:
                                if (selectedLocation != null) {
                                    DeselectLocation();
                                    buildingsUI.SetActive(false);
                                    SoundManager.Play(SoundManager.SoundType.UI, "slider_click_short");
                                }
                                break;

                        }
                    }
                }
            }
        }

        // ### DRAG AND DROP -> moving of troops ###
        if (selectedLocation != null && selectedLocation.tag != "fighting_house") {
            if (IsLocationOwnedByPlayer(selectedLocation)) {
                // Start dragging
                if (Input.GetMouseButtonDown(0)) {
                    if (!WasClickOnUI()) {
                        dragging = true;

                        moveSoldiersNum = 0;
                        foreach (Slider s in selectionSliders) {
                            moveSoldiersNum += (int)s.value;
                        }
                        moveMarkerText.text = moveSoldiersNum.ToString();
                    }
                }

                // Dragging
                if (dragging) {
                    // Update UI
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    // Check for hit valid game objects // Find snapping point
                    bool foundValidGameObject = false;
                    if (Physics.Raycast(ray, out hit)) {
                        GameObject targetLocation = hit.collider.gameObject;
                        if (targetLocation.tag == "game_location") {
                            if (IsSelLocationNeighbour(targetLocation)) {
                                foundValidGameObject = true;
                            }
                        } else if (targetLocation.tag == "fighting_house") {
                            if (GetFightingHouseGameLocationIfValid(targetLocation) != null) foundValidGameObject = true;
                        }
                    }

                    if (foundValidGameObject) {
                        // Found snapping point, snap arrow to collided game object
                        moveTargetMarker.transform.position = hit.collider.transform.Find("Flag").position;
                        moveTargetMarker.SetActive(true);
                        SetMoveMarker(hit.collider.transform.position);
                        SetMoveMarkerTextPos(hit.collider.transform.position);
                    } else {
                        // Move arrow freely
                        // Bit shift the index of the background/Map layer (8) to get a bit mask
                        int layerMask = 1 << 8;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                            Vector3 dragPos = NormalizeVectorInto2D(hit.point);
                            SetMoveMarker(dragPos);
                            SetMoveMarkerTextPos(dragPos);
                        }

                        moveTargetMarker.SetActive(false);
                    }
                }

                // Dropping
                if (dragging && Input.GetMouseButtonUp(0)) {
                    dragging = false;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit)) {
                        GameObject targetLocation = hit.collider.gameObject;
                        if (targetLocation.tag == "game_location") {
                            if (IsSelLocationNeighbour(targetLocation)) {
                                MoveTroops(targetLocation);
                            }
                        } else if (targetLocation.tag == "fighting_house") {
                            targetLocation = GetFightingHouseGameLocationIfValid(targetLocation);
                            if (targetLocation != null) {
                                MoveTroops(targetLocation);
                            }
                        }
                    }
                    moveMarker.SetActive(false);
                    moveTargetMarker.SetActive(false);
                }
            }
        }

        // AI handling
        HandleAI();
    }

    private bool IsLocationOwnedByPlayer(GameObject location) {
        return location.GetComponent<GameLocation>().house.houseType == player.GetComponent<GamePlayer>().house.houseType;
    }

    private void SelectLocation(GameObject targetLocation) {
        selectedLocation = targetLocation;
        selectionMarker.transform.position = selectedLocation.transform.Find("Flag").position;
        selectionMarker.SetActive(true);
        buildingsUI.SetActive(false);
        selectionUI.SetActive(true);
        SoundManager.Play(SoundManager.SoundType.UI, "slider_click");
    }

    public void DeselectLocation() {
        selectedLocation = null;
        selectionMarker.SetActive(false);
        moveMarker.SetActive(false);
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

    /**
        Returns null if the target is not in range.
        Returns the game location game object if the fighting house combat is at a valid location or is on way to a valid location
     */
    private GameObject GetFightingHouseGameLocationIfValid(GameObject target) {
        FightingHouse fh = target.GetComponent<FightingHouse>();
        if (fh.combat.location != null) {
            // Game location combat
            if (IsSelLocationNeighbour(fh.combat.location.gameObject)) {
                return fh.combat.location.gameObject;
            }
        } else {
            // Combat on field
            Troops t = fh.firstParticipant as Troops;
            if (IsSelLocationNeighbour(t.toLocation)) {
                return t.toLocation;
            }
        }
        return null;
    }

    private Vector3 NormalizeVectorInto2D(Vector3 v) {
        v.y = 0f;
        return v;
    }

    private void SetMoveMarker(Vector3 targetPos) {
        moveMarker.SetActive(true);

        // Set anchor to selected location
        Vector3 anchor = selectedLocation.transform.position;
        moveMarker.transform.position = anchor;

        // Set length with distance
        float distanceToDragPos = Vector3.Distance(anchor, targetPos);
        moveMarkerSpriteRenderer.size = new Vector2(distanceToDragPos * 10, moveMarkerSpriteRenderer.size.y); // * 10 because the parent Move Marker Game Object is scaled to 0.1

        // Do rotation with angle
        Vector3 vectorToTarget = targetPos - anchor; // Vector from selected location to touch point
        Vector3 vectorRight = Vector3.right;
        float angle = Vector3.Angle(vectorToTarget, vectorRight);

        // 180 or -180 degrees
        if ((anchor + vectorToTarget).z < anchor.z) {
            angle = -angle;
        }
        moveMarkerSpriteRenderer.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void SetMoveMarkerTextPos(Vector3 targetPos) {
        // Set num soldiers text position to half distance of marker
        Vector3 pos = ((targetPos - selectedLocation.transform.position) * 0.5f) + selectedLocation.transform.position;
        moveMarkerText.transform.position = pos;
    }

    private void MoveTroops(GameObject toLocation) {
        GameLocation gl = selectedLocation.GetComponent<GameLocation>();
        if (gl.numSoldiers > 0) {
            Soldiers moveSoldiers = new Soldiers();
            System.Array soldierTypes = Soldiers.CreateSoldierTypesArray();
            int counter = 0;
            foreach (Slider s in selectionSliders) {
                moveSoldiers.SetSoldierType((SoldierType)counter, gl.soldiers.ExtractSoldiers((SoldierType)counter, (int)s.value));
                counter++;
            }

            if (moveSoldiers.GetNumSoldiersInTotal() > 0) {
                InitializeTroopsMovement(selectedLocation, toLocation, moveSoldiers, true);
                SoundManager.Play(SoundManager.SoundType.UI, "slider_click_short");
            }

            selectionUI.GetComponent<SelectionUI>().SetSoldierSlidersToMax();
        }
    }

    public void InitializeTroopsMovement(GameObject fromLocation, GameObject toLocation, Soldiers soldiers, bool mpSend = true) {
        if (fromLocation.GetComponent<GameLocation>().combat == null) {
            GameObject troopsGO = Instantiate(troopsPrefab, fromLocation.transform.position, fromLocation.transform.rotation);
            troopsGO.name = "Troops " + fromLocation.name + "-" + toLocation.name;
            Troops troops = troopsGO.GetComponent<Troops>();

            GameLocation fromGameLocation = fromLocation.GetComponent<GameLocation>();
            troops.house = fromGameLocation.house;
            troops.soldiers = soldiers;
            fromGameLocation.UpdateGUI();
            troops.fromLocation = fromLocation;
            troops.toLocation = toLocation;

            if (Multiplayer.NetworkManager.mpActive && mpSend) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCMoveTroops(fromGameLocation, toLocation.GetComponent<GameLocation>(), soldiers));

            AIPlayer.InformOfMovingTroops(troops);
        }
    }

    public void OpenBuildingsMenu() {
        selectionUI.SetActive(false);
        buildingsUI.SetActive(true);
    }

    /**
        Call this in a destructor or similar in GameObjects, that can be selected
     */
    public static void DeselectLocationOnDisable(GameObject disabledGameObject) {
        if (GameController.activeGameController.selectedLocation == disabledGameObject) {
            GameController.activeGameController.DeselectLocation();
        }
    }

    private bool WasClickOnUI() {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        else {
            if (Input.touchCount > 0) {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
            }
        }
        return false;
    }

    // Needed because a selected loaction on quit caused a little error (not too important, but bothering)
    private void OnApplicationQuit() {
        DeselectLocation();
    }

    private void HandleAI() {
        if (aiPlayers.Count > 0) {
            aiElapsedTime += Time.deltaTime;
            if (aiElapsedTime >= Global.GAME_CONTROLLER_AI_HANDLE_CYCLE_TIME / (float)aiPlayers.Count) {
                aiElapsedTime = 0f;

                if (aiPlayers[aiPlayerCounter].ownedLocations.Count <= 0) {
                    aiPlayers.RemoveAt(aiPlayerCounter);
                    if (aiPlayers.Count > 0) aiPlayerCounter = (aiPlayerCounter + 1) % aiPlayers.Count;
                } else {
                    aiPlayers[aiPlayerCounter].Play();
                    aiPlayerCounter = (aiPlayerCounter + 1) % aiPlayers.Count;
                }
            }
        }
    }

    /// <summary>Returns a string with Jsonified SaveGameLocationData of all locations seperated by #</summary>
    public string FastSaveGame() {
        string res = "";
        foreach (GameLocation gl in GameLocation.allGameLocations) {
            res += JsonUtility.ToJson(new SaveGameLocationData(gl)) + "#";
        }
        // Replace closing brackets because of the multiple commands per tcp read occurence
        res = res.Replace('}', '>');
        // Remove last #
        res = res.Remove(res.Length - 1);
        return res;
    }

    /// <summary>Handles savegame by FastSaveGame()</summary>
    public void HandleFastSaveGameData(string data) {
        // Restore original JSON data
        data = data.Replace('>', '}');
        Global.SAVE_GAME = data;

        GamePlayer p = player.GetComponent<GamePlayer>();
        Global.SAVE_GAME_GOLD = p.house.gold;
        Global.SAVE_GAME_MP = p.house.manpower;

        foreach (AIPlayer ai in aiPlayers) {
            Global.SAVE_GAME_HOUSE_DATA.Add(new SaveGameHouseData(ai.house, ai.goldPool));
        }

        House.ResetHouses();
        GameLocation.allGameLocations.Clear();
        if (Multiplayer.NetworkManager.mpActive) Multiplayer.NetworkManager.mpActions.Clear();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void LoadSaveGame() {
        string[] rawLocationData = Global.SAVE_GAME.Split('#');
        List<SaveGameLocationData> locationData = new List<SaveGameLocationData>();

        try {
            foreach (string s in rawLocationData) {
                locationData.Add(JsonUtility.FromJson<SaveGameLocationData>(s));
            }
        } catch (System.Exception e) {
            Debug.LogError("Could not parse location data while loading a savegame: " + e);
            Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncGame(true));
            return;
        }

        // Setup saved game state
        SaveGameLocationData ld = null;
        foreach (GameLocation gl in GameLocation.allGameLocations) {
            ld = locationData.Find(curr => curr.n == gl.locationName);

            if (ld != null) {
                if (ld.h != (int)gl.house.houseType) gl.OccupyBy(House.FindHouseByType((HouseType)ld.h), true);

                gl.buildings.Clear();
                if (ld.b.Length > 0) {
                    foreach (int btInt in ld.b) {
                        gl.AddBuilding(Building.CreateBuildingInstance((BuildingType)btInt), false);
                    }
                } else {
                    gl.GetEffectsFromBuildings();
                    gl.DetermineFortificationLevel();
                }

                gl.soldiers = Multiplayer.NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(ld.s);
                Soldiers recSoldiers = Multiplayer.NetworkCommands.NetworkCommand.SoldiersNumsArrayToObj(ld.r);
                if (recSoldiers.GetNumSoldiersInTotal() > 0) gl.AddSoldiersToRecruitment(recSoldiers, false);
            } else {
                Debug.LogError($"Could not find location data for <{gl.locationName}> while loading a save game");
                Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncGame(true));
                return;
            }
        }

        GamePlayer p = player.GetComponent<GamePlayer>();
        p.house.gold = Global.SAVE_GAME_GOLD;
        p.house.manpower = Global.SAVE_GAME_MP;

        SaveGameHouseData hd = null;
        foreach (AIPlayer ai in aiPlayers) {
            hd = Global.SAVE_GAME_HOUSE_DATA.Find(curr => curr.houseTypeInt == (int)ai.house.houseType);

            if (hd != null) {
                ai.house.gold = hd.gold;
                ai.house.manpower = hd.mp;
            } else {
                Debug.LogError($"Could not find house data for <{ai.house.houseName}> while loading a save game");
                Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncGame(true));
                return;
            }
        }

        Global.SAVE_GAME = null;
        Global.SAVE_GAME_GOLD = 0;
        Global.SAVE_GAME_MP = 0;
        Global.SAVE_GAME_HOUSE_DATA.Clear();

        ConnInfoPanel.instance.ShowPanel("Game resynced");
        Debug.Log("Game resynced");
    }
}