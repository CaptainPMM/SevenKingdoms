using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class FightingHouse : Combatable {
    public static List<FightingHouse> allFightingHouses = new List<FightingHouse>();

    public GameObject casualtiesPopupPrefab;

    /// <summary>If the fighting house originates from a location, this is simply the location name.
    /// If the firstParticipant was a Troops object the ID is generated like this:
    /// (houseType as int)#(soldiers numbers of every type seperated by ,)#(fromLocation)#(toLocation)
    /// E.g. 2#0,0,0,10,5,#Castle Castle Black#Outpost Last Hearth</summary>
    public string ID {
        get {
            return id;
        }
    }
    [SerializeField] private string id;

    public HouseType houseType {
        get {
            return house.houseType;
        }
    }
    public Combatable firstParticipant {
        get {
            return _firstParticipant;
        }
    }
    public GameLocation location {
        get {
            return _location;
        }
    }
    public List<GameEffect> locationEffects {
        get {
            return _locationEffects;
        }
    }

    [SerializeField] private Combatable _firstParticipant; // Set this after instantiating
    [SerializeField] private GameLocation _location; // Set this after instantiating
    [SerializeField] private List<GameEffect> _locationEffects;

    private Dictionary<SoldierType, int> deadSoldiersPerTick;

    /**
        This has to be called before adding participants
     */
    public void Init(Combatable firstParticipant, GameLocation location) {
        _firstParticipant = firstParticipant;
        _location = location;
        house = firstParticipant.house;
        deadSoldiersPerTick = new Dictionary<SoldierType, int>();
        GetComponentInChildren<SpriteRenderer>().color = house.color;
        soldiers = firstParticipant.soldiers;
        combat = firstParticipant.combat;
        firstParticipant.gameObject.SetActive(false); // Hide first participant for the duration of combat

        if (location != null) {
            if (location.house.houseType == houseType) {
                // This house is the defender of a game location -> effects?
                _locationEffects = location.locationEffects.FindAll(ge => ge.type == GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS);
            } else {
                _locationEffects = null;
            }

            id = location.name;
        } else {
            // Create soldiers string for id
            string soldiersString = "";
            foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                soldiersString += firstParticipant.soldiers.GetSoldierTypeNum(st) + ",";
            }

            // If the location is null the Combatable must be of type Troops -> has fromLocation and toLocation
            Troops t = (Troops)firstParticipant;

            id = $"{(int)houseType}#{soldiersString}#{t.fromLocation.name}#{t.toLocation.name}";
        }

        allFightingHouses.Add(this);
    }

    void Update() {
        if (deadSoldiersPerTick.Count > 0) {
            int oldNumSoldiers = numSoldiers;
            int casualties = 0;

            foreach (KeyValuePair<SoldierType, int> curr in deadSoldiersPerTick) {
                casualties += soldiers.DealDamageToSoldierType(curr.Key, curr.Value);
            }
            deadSoldiersPerTick.Clear();

            GameObject go = Instantiate(casualtiesPopupPrefab);
            go.name = "CasualtiesPopup: " + casualties;
            Vector3 pos = transform.position;
            pos.z += 1.16f; // hard coded :/
            pos.x += 0.04f; // hard coded :/
            go.transform.position = pos;
            go.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Min(oldNumSoldiers, casualties).ToString();

            UpdateGUI();
        }
    }

    public void AddParticipant(Combatable participant) {
        soldiers.AddSoldiers(new Soldiers(participant.soldiers, true));
        UpdateGUI();
        Destroy(participant.gameObject);
    }

    public void ApplyCasualties(SoldierType soldierType, int amount) {
        if (deadSoldiersPerTick.ContainsKey(soldierType)) {
            deadSoldiersPerTick[soldierType] += amount;
        } else {
            deadSoldiersPerTick.Add(soldierType, amount);
        }
    }

    public void CombatIsOver() {
        if (_firstParticipant != null) {
            _firstParticipant.gameObject.SetActive(true);
            //_firstParticipant.soldiers = soldiers; // Not needed anymore, because soldiers is an object and already connected (leave it though)
            _firstParticipant.combat = null;
            _firstParticipant.UpdateGUI();
        }
        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        allFightingHouses.Remove(this);
    }
}