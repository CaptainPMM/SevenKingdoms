using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPlayer {
    public House house;
    public List<GameLocation> ownedLocations;

    private Dictionary<SoldierType, int> soldierTypePredictedStrengths;

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
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) soldierTypePredictedStrengths.Add(st, Soldiers.GetSoldierTypeStats(st).predictedStrength);
    }

    public void Play() {
        TroopsManagement();
    }

    private void TroopsManagement() {
        foreach (GameLocation ownGameLocation in ownedLocations) {
            if (ownGameLocation.numSoldiers > 0) {
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
                        float strengthRatio = CompareStrength(ownGameLocation, attackLocation);

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

    /**
        Returns a value below zero if origin is more powerful; or above zero if target is more powerful.
        Returns 0 if both are equally powerful.
        The value increases with greater difference.
     */
    private float CompareStrength(GameLocation origin, GameLocation target) {
        float strengthRatio = 0f;

        float targetLocationEffectsMod = 1f;
        foreach (GameEffect ge in target.locationEffects)
            if (ge.type == GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS)
                targetLocationEffectsMod *= ge.modifierValue;
        float targetStrengthBonus = 1f + (1f - targetLocationEffectsMod);

        int originSoldiersPredictedStrength = 0;
        int targetSoldiersPredictedStrength = 0;
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            originSoldiersPredictedStrength += origin.soldiers.GetSoldierTypeNum(st) * soldierTypePredictedStrengths[st];
            targetSoldiersPredictedStrength += target.soldiers.GetSoldierTypeNum(st) * soldierTypePredictedStrengths[st];
        }

        float originSoldiersMod = (float)originSoldiersPredictedStrength / (float)origin.numSoldiers;
        float targetSoldiersMod = (float)targetSoldiersPredictedStrength / (float)target.numSoldiers;

        strengthRatio = ((float)target.numSoldiers * targetStrengthBonus * targetSoldiersMod) - ((float)origin.numSoldiers * originSoldiersMod);

        return strengthRatio;
    }
}