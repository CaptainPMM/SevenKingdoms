using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {
    public GameObject fightingHousePrefab;

    public GameLocation location;

    [SerializeField] private List<FightingHouse> fightingHouses;
    [SerializeField] private bool inProgress;
    private FightingHouse defeatedLocation = null;

    private float elapsedTime;

    /**
        Call this first after instantiating
     */
    public void Init(GameLocation combatLocation) {
        location = combatLocation;
        fightingHouses = new List<FightingHouse>();
        inProgress = false;
        elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update() {
        if ((!Multiplayer.NetworkManager.mpActive || Multiplayer.NetworkManager.isServer) && inProgress) {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= Global.COMBAT_SPEED) {
                elapsedTime = 0;

                if (DetermineCombatStatus() == true) {
                    CalcCasualties();
                }
            }
        }
    }

    public void AddParticipant(Combatable newParticipant) {
        FightingHouse fightingHouse = fightingHouses.Find(fh => fh.houseType == newParticipant.house.houseType);
        if (fightingHouse != null) {
            // Add new participant to exisitng fighting house
            fightingHouse.AddParticipant(newParticipant);
        } else {
            // Instantiate new fighting house with the new participant
            GameObject go = Instantiate(fightingHousePrefab);
            go.name = "FightingHouse " + newParticipant.house.houseName;
            go.transform.position = newParticipant.transform.position;
            FightingHouse fh = go.GetComponent<FightingHouse>();
            if (newParticipant.GetType().IsEquivalentTo(typeof(Troops))) {
                // Attacking house with no game location bonus
                fh.Init(newParticipant, null);
            } else {
                // Defending house with game location bonus
                fh.Init(newParticipant, location);
            }
            fightingHouses.Add(fh);
        }
    }

    public void BeginCombat() {
        inProgress = true;
    }

    void CalcCasualties() {
        // For every house pick a random target house and deal damage
        for (int currIndex = 0; currIndex < fightingHouses.Count; currIndex++) {
            int targetHouseIndex = -1;

            if (fightingHouses.Count == 2) {
                // Speed up target determination
                targetHouseIndex = (currIndex + 1) % 2; // 0 attack 1, 1 attack 0
            } else {
                do {
                    int randomIndex = PickRandomHouseTargetIndex(fightingHouses);
                    if (randomIndex != currIndex) {
                        targetHouseIndex = randomIndex;
                    }
                } while (targetHouseIndex == -1);
            }

            FightingHouse attacker = fightingHouses[currIndex];
            FightingHouse defender = fightingHouses[targetHouseIndex];

            // Damage calculation
            // For each soldier type select a random target soldier type (random is influenced by soldier type number)
            foreach (SoldierType attackingST in Soldiers.CreateSoldierTypesArray()) {
                // Only if the attacking house curr soldier type has soldiers
                if (attacker.soldiers.GetSoldierTypeNum(attackingST) > 0) {
                    int rand = Random.Range(1, defender.numSoldiers + 1);
                    SoldierType targetSoldierType = SoldierType.CONSCRIPTS; // Default value
                    int passedSoldiersCounter = 0;

                    foreach (SoldierType defendingST in Soldiers.CreateSoldierTypesArray()) {
                        passedSoldiersCounter += defender.soldiers.GetSoldierTypeNum(defendingST);

                        if (rand <= passedSoldiersCounter) {
                            // Random number has selected the current soldier type (defendingST)
                            targetSoldierType = defendingST;
                            break;
                        }
                    }

                    // Deal damage
                    Soldier attackingST_stats = Soldiers.GetSoldierTypeStats(attackingST);

                    int ATTACKING_SOLDIER_TYPE_NUM = attacker.soldiers.GetSoldierTypeNum(attackingST);
                    int ATTACKING_SOLDIER_TYPE_DP = attackingST_stats.DP;

                    float ATTACKING_BONUS_AGAINST_DEFENING_SOLDIER_TYPE = 1f; // Neutral bonus -> no effects
                    if (!attackingST_stats.strengths.TryGetValue(targetSoldierType, out ATTACKING_BONUS_AGAINST_DEFENING_SOLDIER_TYPE)) {
                        // No bonus against the defending soldier type found -> neutral bonus
                        ATTACKING_BONUS_AGAINST_DEFENING_SOLDIER_TYPE = 1f;
                    }

                    float DEFENDER_BONUS = 1f; // Neutral bonus -> no effects
                    if (defender.locationEffects != null) {
                        foreach (GameEffect ge in defender.locationEffects) {
                            DEFENDER_BONUS *= ge.modifierValue;
                        }
                    }

                    float RANDOM = Random.Range(Global.COMBAT_DAMAGE_RAND_MIN, Global.COMBAT_DAMAGE_RAND_MAX);

                    // Combine all damage factors
                    int damage = Mathf.RoundToInt(
                                    1 +
                                    (
                                        (float)ATTACKING_SOLDIER_TYPE_NUM *
                                        (float)ATTACKING_SOLDIER_TYPE_DP *
                                        ATTACKING_BONUS_AGAINST_DEFENING_SOLDIER_TYPE *
                                        DEFENDER_BONUS *
                                        Global.COMBAT_DAMAGE_DAMPER *
                                        RANDOM
                                    )
                                );

                    defender.ApplyCasualties(targetSoldierType, damage);
                    if (Multiplayer.NetworkManager.mpActive) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncCombat(defender, targetSoldierType, damage));
                }
            }
        }
    }

    int PickRandomHouseTargetIndex(List<FightingHouse> houses) {
        return Random.Range(0, houses.Count);
    }

    /**
        Returns true if combat still running or false if its over
     */
    public bool DetermineCombatStatus() {
        FightingHouse mpSendFightingHouse = fightingHouses[0]; // Save one fighting house in case the combat is over and the server has to send a participant to his clients for ending

        // Check for fighting houses with no more soldiers and remove them
        List<FightingHouse> removeFightingHouses = new List<FightingHouse>();
        for (int i = 0; i < fightingHouses.Count; i++) {
            FightingHouse fh = fightingHouses[i];
            if (fh.numSoldiers <= 0) {
                removeFightingHouses.Add(fh);

                if (fh.location == null) {
                    // Only remove troops, no game locations
                    if (fh.firstParticipant != null) Destroy(fh.firstParticipant.gameObject);
                    Destroy(fh.gameObject);
                } else {
                    // Combat is over for game location
                    defeatedLocation = fh;
                }
            }
        }
        foreach (FightingHouse fh in removeFightingHouses) {
            fightingHouses.Remove(fh);
        }

        // Check if only 1 or less fighting houses have soldiers left
        if (fightingHouses.Count <= 1) {
            // Combat is over
            FightingHouse winner = null;
            if (fightingHouses.Count > 0) { // With this branch we are safe in the case: nobody has soldiers left, and fightingHouses[0] is safe
                winner = fightingHouses[0];
                winner.CombatIsOver();

                if (location != null) {
                    // Siege on game location
                    if (winner.houseType != location.house.houseType) {
                        // Attackers won (occupy game location)
                        location.OccupyBy(winner.house);
                    }
                }
            }

            if (defeatedLocation != null) {
                defeatedLocation.CombatIsOver();
            }

            if (Multiplayer.NetworkManager.isServer) {
                if (winner != null) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncCombatEnd(winner));
                else Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCSyncCombatEnd(mpSendFightingHouse)); // In case there is no winner
            }

            Destroy(this.gameObject);
            return false;
        } else {
            return true;
        }
    }
}
