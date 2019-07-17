using UnityEngine;

public class Troops : Combatable {
    public GameObject combatPrefab;

    public GameObject toLocation;

    [SerializeField] private float moveSpeed;

    // Start is called before the first frame update
    new void Start() {
        base.Start();

        moveSpeed = Global.TROOPS_BASE_MOVE_SPEED * Random.Range(Global.TROOPS_MOVE_SPEED_RAND_MIN, Global.TROOPS_MOVE_SPEED_RAND_MAX);
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, toLocation.transform.position, moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.Equals(toLocation)) {
            // Reached destination location
            GameLocation destination = other.GetComponent<GameLocation>();
            if (destination.house.houseType == house.houseType) {
                // Peacfully put troops in location (Reinforcements)
                destination.soldiers.AddSoldiers(soldiers);
                destination.UpdateGUI();
                Destroy(gameObject);
            } else {
                // Location combat (Siege)
                InitCombat(other, destination);
            }
            return;
        }

        if (other.gameObject.tag == "troops") {
            // Hit other troops on the road
            if (other.GetComponent<Troops>().house.houseType != house.houseType) {
                InitCombat(other, null);
            }
            return;
        }

        if (other.gameObject.tag == "fighting_house") {
            // Combat exists
            AddToCombat(other);
        }
    }

    private void InitCombat(Collider other, GameLocation location) {
        Combatable otherCombatable = other.GetComponent<Combatable>();
        if (otherCombatable.combat == null) {
            GameObject combatGO = Instantiate(combatPrefab);
            combatGO.name = "Combat " + house.houseName + "-" + other.GetComponent<Combatable>().house.houseName;
            Vector3 pos = (transform.position + other.transform.position) / 2; // Midpoint between both gameobjects
            pos.z += 0.75f;
            combatGO.transform.position = pos;

            combat = combatGO.GetComponent<Combat>();
            combat.Init(location);
            combat.AddParticipant(this);
            if (location != null) {
                // Add game locations to the combat, troops add themselfs automatically
                otherCombatable.combat = combat;
                combat.AddParticipant(otherCombatable);
            }
            combat.BeginCombat();
        } else {
            // Add this troops to the existing combat
            combat = otherCombatable.combat;
            combat.AddParticipant(this);
        }
    }

    private void AddToCombat(Collider other) {
        FightingHouse fh = other.GetComponent<FightingHouse>();
        combat = fh.combat;
        combat.AddParticipant(this);
    }

    private void OnDestroy() {
        AIPlayer.RemoveMovingTroops(this);
    }
}