using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPlayer {
    public House house;
    public List<GameLocation> ownedLocations;

    private Dictionary<SoldierType, int> soldierTypePredictedStrengths;
    private static List<Troops> troops = new List<Troops>();

    public AIPlayer(HouseType houseType) {
        this.house = new House(houseType);

        // Find all owned locations
        ownedLocations = new List<GameLocation>();
        GameObject[] gameLocationGOs = GameObject.FindGameObjectsWithTag("game_location");
        foreach (GameObject gameLocationGO in gameLocationGOs) {
            GameLocation gameLocation = gameLocationGO.GetComponent<GameLocation>();
            if (gameLocation.house.houseType == house.houseType) {
                ownedLocations.Add(gameLocation);
            }
        }

        // Save soldier type stats for later
        soldierTypePredictedStrengths = new Dictionary<SoldierType, int>();
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) soldierTypePredictedStrengths.Add(st, Soldiers.GetSoldierTypeStats(st).predictedStrength);
    }

    public void Play() {
        TroopsManagement();
    }

    private void TroopsManagement() {
        foreach (GameLocation ownGameLocation in ownedLocations) {
            if (ownGameLocation.numSoldiers > 0) {
                // If enemy troops are attacking this location, stay and defend or flee
                Soldiers enemySoldiersAttacking = new Soldiers();
                foreach (Troops t in troops) {
                    if (t.house.houseType != house.houseType && t.toLocation == ownGameLocation.gameObject) {
                        enemySoldiersAttacking.AddSoldiers(t.soldiers);
                    }
                }

                if (enemySoldiersAttacking.GetNumSoldiersInTotal() > 0) {
                    if (CompareStrength(enemySoldiersAttacking, ownGameLocation) < 0) {
                        // Flee to friendly neighbour location (if possible a castle)
                        GameLocation target = FindPreferredFriendlyLocation(ownGameLocation.reachableLocations);
                        if (target != null) AIGameActions.MoveTroops(ownGameLocation, target);
                    }
                    // Else stay and defend
                } else {
                    // Determine if enemies are nearby
                    List<GameLocation> potentialAttackLocations = new List<GameLocation>();
                    foreach (GameLocation neighbourGameLocation in ownGameLocation.reachableLocations) {
                        if (neighbourGameLocation.house.houseType != this.house.houseType) {
                            potentialAttackLocations.Add(neighbourGameLocation);
                        }
                    }

                    if (potentialAttackLocations.Count > 0) {
                        // Probably attack one of the potential locations
                        List<GameLocation> reasonableAttackLocations = new List<GameLocation>();
                        foreach (GameLocation attackLocation in potentialAttackLocations) {
                            float strengthRatio = CompareStrength(ownGameLocation.soldiers, attackLocation);

                            if (strengthRatio < 0) {
                                // Own location is stronger, add to reasonable attack locations
                                reasonableAttackLocations.Add(attackLocation);
                            }
                        }

                        // Determine final attack location if any is reasonable (castles are preferred)
                        if (reasonableAttackLocations.Count > 0) {
                            List<GameLocation> castles = new List<GameLocation>();
                            List<GameLocation> outposts = new List<GameLocation>();

                            foreach (GameLocation gl in reasonableAttackLocations) {
                                (gl.GetType() == typeof(Castle) ? castles : outposts).Add(gl);
                            }

                            if (castles.Count > 0) {
                                AIGameActions.MoveTroops(ownGameLocation, castles[Random.Range(0, castles.Count)]);
                            } else {
                                AIGameActions.MoveTroops(ownGameLocation, outposts[Random.Range(0, outposts.Count)]);
                            }
                        }
                    } else {
                        // Move troops around
                        AIGameActions.MoveTroops(ownGameLocation, ownGameLocation.reachableLocations[Random.Range(0, ownGameLocation.reachableLocations.Length)]);
                    }
                }
            }
        }
    }

    /**
        Returns a value below zero if attacker is more powerful; or above zero if defender is more powerful.
        Returns 0 if both are equally powerful.
        The value increases with greater difference.
        !! The defender combatable can have defender bonus modifiers like walls !!
     */
    private float CompareStrength(Soldiers attacker, Combatable defender) {
        float strengthRatio = 0f;

        float defenderStrengthBonus = 1f;
        if (defender.GetType().IsSubclassOf(typeof(GameLocation))) {
            float defenderLocationEffectsMod = 1f;
            foreach (GameEffect ge in ((GameLocation)defender).locationEffects)
                if (ge.type == GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS)
                    defenderLocationEffectsMod *= ge.modifierValue;
            defenderStrengthBonus = 1f + (1f - defenderLocationEffectsMod);
        }

        int attackerSoldiersPredictedStrength = 0;
        int defenderSoldiersPredictedStrength = 0;
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            attackerSoldiersPredictedStrength += attacker.GetSoldierTypeNum(st) * soldierTypePredictedStrengths[st];
            defenderSoldiersPredictedStrength += defender.soldiers.GetSoldierTypeNum(st) * soldierTypePredictedStrengths[st];
        }

        strengthRatio = ((float)defenderSoldiersPredictedStrength * defenderStrengthBonus) - (float)attackerSoldiersPredictedStrength;

        return strengthRatio;
    }

    private GameLocation FindPreferredFriendlyLocation(GameLocation[] locations) {
        List<GameLocation> castles = new List<GameLocation>();
        List<GameLocation> outposts = new List<GameLocation>();

        foreach (GameLocation gl in locations) {
            if (gl.house.houseType == house.houseType) {
                (gl.GetType() == typeof(Castle) ? castles : outposts).Add(gl);
            }
        }

        if (castles.Count > 0) {
            return castles[Random.Range(0, castles.Count)];
        } else {
            if (outposts.Count > 0) return outposts[Random.Range(0, outposts.Count)];
            return null;
        }
    }

    public static void InformOfMovingTroops(Troops t) {
        troops.Add(t);
    }

    public static void RemoveMovingTroops(Troops t) {
        troops.Remove(t);
    }
}