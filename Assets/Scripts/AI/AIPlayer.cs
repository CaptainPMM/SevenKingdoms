using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPlayer {
    public House house;
    public List<GameLocation> ownedLocations;

    private Dictionary<SoldierType, int> soldierTypePredictedStrengths;
    private static List<Troops> troops = new List<Troops>();

    private AIGoldPool goldPool; // only work with this gold in AI, normal house.gold doesnt work here!!

    private Dictionary<SoldierType, int> requiredBuildingsForSoldierType;

    // Soldier type priorities
    private Dictionary<SoldierType, int> soldierTypePriorities = new Dictionary<SoldierType, int>() {
        { SoldierType.CONSCRIPTS, 2 },
        { SoldierType.SPEARMEN, 2 },
        { SoldierType.SWORDSMEN, 3 },
        { SoldierType.BOWMEN, 2 },
        { SoldierType.CAV_KNIGHTS, 4 }
    };

    // Building type priorities
    private Dictionary<BuildingType, int> buildingTypePriorities = new Dictionary<BuildingType, int>() {
        { BuildingType.LOCAL_ADMINISTRATION, 1 }, // only in outposts because in castles they get built with highest prio already
        { BuildingType.MARKETPLACE, 5 },
        { BuildingType.OUTER_TOWN_RING, 7 },
        { BuildingType.WOODEN_WALL, 1 },
        { BuildingType.STONE_WALL, 4 },
        { BuildingType.ADVANCED_WALL, 12 },
        { BuildingType.WOOD_MILL, 6 },
        { BuildingType.BOW_MAKER, 4 },
        { BuildingType.BLACKSMITH, 6 },
        { BuildingType.STABLES, 4 },
        { BuildingType.BARRACKS, 1 },
        { BuildingType.DRILL_GROUND, 10 }
    };
    private int buildLocalAdminInSafeOutpostsCounter = 0;

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

        goldPool = new AIGoldPool();
        DistributeGold();

        requiredBuildingsForSoldierType = new Dictionary<SoldierType, int>();
        foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
            int requiredBuildings = 0;
            foreach (BuildingType bt in System.Enum.GetValues(typeof(BuildingType))) {
                GameEffect[] effects = Building.GetBuildingTypeInfos(bt).gameEffects;
                foreach (GameEffect e in effects) {
                    if (e.type == GameEffectType.SOLDIER_TYPE_UNLOCK && e.modifierValue == (int)st) {
                        requiredBuildings++;
                        break;
                    }
                }
            }
            requiredBuildingsForSoldierType.Add(st, requiredBuildings);
        }
    }

    public void Play() {
        TroopsManagement();
        ResourceManagement();
    }

    private void TroopsManagement() {
        foreach (GameLocation ownGameLocation in ownedLocations) {
            if (ownGameLocation.numSoldiers > 0) {
                // Determine garrison soldier amount
                float locationDefendingBonus = 1f;
                if (ownGameLocation.GetType() == typeof(Castle)) locationDefendingBonus = 0.8f; // Castle base garrison
                foreach (GameEffect ge in ownGameLocation.locationEffects) {
                    if (ge.type == GameEffectType.COMBAT_LOCATION_DEFENDER_BONUS) locationDefendingBonus *= ge.modifierValue;
                }

                if (locationDefendingBonus < 1f) {
                    int garrisonNum = Mathf.RoundToInt((1f - locationDefendingBonus) * 100f); // Soldier amount to stay
                    float soldiersStayPerType = Mathf.Min((float)garrisonNum / (float)ownGameLocation.soldiers.GetNumSoldiersInTotal(), 1f);
                    Soldiers staySoldiers = new Soldiers();
                    foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                        staySoldiers.SetSoldierTypeNum(st, Mathf.FloorToInt((float)ownGameLocation.soldiers.GetSoldierTypeNum(st) * soldiersStayPerType));
                    }
                    Soldiers moveSoldiers = new Soldiers(ownGameLocation.soldiers);
                    moveSoldiers.RemoveSoldiers(staySoldiers);

                    // Move only soldiers not garrisoned
                    DoTroopsMovement(ownGameLocation, moveSoldiers, true);
                } else {
                    // Move all soldiers
                    DoTroopsMovement(ownGameLocation, ownGameLocation.soldiers, false);
                }
            }
        }
    }

    private void DoTroopsMovement(GameLocation from, Soldiers soldiers, bool dontFlee) {
        // If enemy troops are attacking this location, stay and defend or flee
        Soldiers enemySoldiersAttacking = new Soldiers();
        foreach (Troops t in troops) {
            if (t.house.houseType != house.houseType && t.toLocation == from.gameObject) {
                enemySoldiersAttacking.AddSoldiers(t.soldiers);
            }
        }

        if (enemySoldiersAttacking.GetNumSoldiersInTotal() > 0) {
            if (!dontFlee && CompareStrength(enemySoldiersAttacking, from) < 0) {
                // Flee to friendly neighbour location (if possible a castle)
                GameLocation target = FindPreferredFriendlyLocation(from.reachableLocations);
                if (target != null) AIGameActions.MoveTroops(from, target);
            }
            // Else stay and defend
        } else {
            // Determine if enemies are nearby
            List<GameLocation> potentialAttackLocations = new List<GameLocation>();
            foreach (GameLocation neighbourGameLocation in from.reachableLocations) {
                if (neighbourGameLocation.house.houseType != this.house.houseType) {
                    potentialAttackLocations.Add(neighbourGameLocation);
                }
            }

            if (potentialAttackLocations.Count > 0) {
                // Probably attack one of the potential locations
                List<GameLocation> reasonableAttackLocations = new List<GameLocation>();
                foreach (GameLocation attackLocation in potentialAttackLocations) {
                    float strengthRatio = CompareStrength(soldiers, attackLocation);

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
                        AIGameActions.MoveTroops(from, castles[Random.Range(0, castles.Count)], soldiers);
                    } else {
                        AIGameActions.MoveTroops(from, outposts[Random.Range(0, outposts.Count)], soldiers);
                    }
                }
            } else {
                // Move troops around
                AIGameActions.MoveTroops(from, from.reachableLocations[Random.Range(0, from.reachableLocations.Length)], soldiers);
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

    private void ResourceManagement() {
        // Update gold pool
        DistributeGold();

        ManageRecruitment();
        ManageBuilding();
    }

    private void DistributeGold() {
        if (house.gold > 0) {
            int newGold = house.gold;

            // Gold pool exchange if needed
            float recruitBuildGoldRatio = (goldPool.recruitmentGold + 1) / (goldPool.buildingGold + 1); // +1 to prevent 0 values
            if (recruitBuildGoldRatio > 1.5f) {
                // Only add gold to build pool
                goldPool.buildingGold += newGold;
            } else if (recruitBuildGoldRatio < 0.35f) {
                // Only add gold to recruit pool
                goldPool.recruitmentGold += newGold;
            } else {
                // Normal gold distribution (shared)
                int recruitmentGold = Mathf.RoundToInt((float)newGold * 0.5f);
                goldPool.recruitmentGold += recruitmentGold;
                int buildingGold = newGold - recruitmentGold;
                goldPool.buildingGold += buildingGold;
            }

            house.gold = 0;
        }
    }

    private void ManageRecruitment() {
        if (house.manpower > 0 && goldPool.recruitmentGold >= 25) {
            // Search location where nothing gets recruited // prefer castles
            List<GameLocation> outposts = new List<GameLocation>();

            int maxRecruitBuildings = 0;
            GameLocation bestCastle = null;

            foreach (GameLocation gl in ownedLocations) {
                if (gl.GetSoldiersInRecruitment().GetNumSoldiersInTotal() > 0) continue; // is already recruiting -> skip

                if (gl.GetType() == typeof(Outpost)) {
                    outposts.Add(gl);
                    continue;
                }

                // Is castle
                int recruitBuildings = 0;
                foreach (Building b in gl.buildings) {
                    foreach (GameEffect ge in b.gameEffects) {
                        if (ge.type == GameEffectType.SOLDIER_TYPE_UNLOCK) recruitBuildings++;
                        if (ge.type == GameEffectType.RECRUITMENT_SPEED) recruitBuildings += 2;
                    }
                }
                if (recruitBuildings > maxRecruitBuildings) {
                    maxRecruitBuildings = recruitBuildings;
                    bestCastle = gl;
                }
            }

            if (bestCastle != null) {
                // Recruit in castle
                RecruitInLocation(bestCastle);
            } else {
                if (outposts.Count > 0) {
                    // Recruit in best outpost
                    List<GameLocation> goodOutposts = new List<GameLocation>();

                    foreach (GameLocation o in outposts) {
                        foreach (Building b in o.buildings) {
                            foreach (GameEffect ge in b.gameEffects) {
                                if (ge.type == GameEffectType.RECRUITMENT_SPEED) goodOutposts.Add(o);
                            }
                        }
                    }

                    if (goodOutposts.Count > 0) {
                        RecruitInLocation(goodOutposts[Random.Range(0, goodOutposts.Count)]);
                    } else {
                        // Recruit in any outpost
                        RecruitInLocation(outposts[Random.Range(0, outposts.Count)]);
                    }
                }
                // Oh no, no suitable recruitment place found
            }
        }
    }

    private void RecruitInLocation(GameLocation gl) {
        if (gl.GetType() == typeof(Outpost)) {
            int maxAvailSoldiersByGold = Mathf.FloorToInt((float)goldPool.recruitmentGold / (float)Soldiers.GetSoldierTypeStats(SoldierType.CONSCRIPTS).goldCosts);
            int maxAvailSoldiersByMP = house.manpower;
            int numToRecruit = Mathf.Min(maxAvailSoldiersByGold, maxAvailSoldiersByMP);

            if (numToRecruit > 0) {
                goldPool.recruitmentGold -= numToRecruit * Soldiers.GetSoldierTypeStats(SoldierType.CONSCRIPTS).goldCosts;
                house.manpower -= numToRecruit;
                Soldiers soldiers = new Soldiers();
                soldiers.SetSoldierTypeNum(SoldierType.CONSCRIPTS, numToRecruit);
                AIGameActions.Recruit(gl, soldiers);
            }
        } else {
            // Is castle
            // Determine recruitable soldier types in gl
            int[] buildingsPerSoldierType = new int[Soldiers.CreateSoldierTypesArray().Length]; // by soldier type enum index
            foreach (Building b in gl.buildings) {
                foreach (GameEffect ge in b.gameEffects) {
                    if (ge.type == GameEffectType.SOLDIER_TYPE_UNLOCK) buildingsPerSoldierType[(int)ge.modifierValue]++;
                }
            }
            List<SoldierType> recruitableSoldierTypes = new List<SoldierType>();
            foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                if (buildingsPerSoldierType[(int)st] >= requiredBuildingsForSoldierType[st]) {
                    // Soldier type is recruitable here
                    recruitableSoldierTypes.Add(st);
                }
            }

            int randMax = 0;
            foreach (SoldierType st in recruitableSoldierTypes) {
                randMax += soldierTypePriorities[st];
            }

            if (randMax > 0) {
                int rand = Random.Range(0, randMax);

                int counter = 0;
                foreach (SoldierType st in recruitableSoldierTypes) {
                    counter += soldierTypePriorities[st];

                    if (counter > rand) {
                        int maxAvailSoldiersByGold = Mathf.FloorToInt((float)goldPool.recruitmentGold / (float)Soldiers.GetSoldierTypeStats(st).goldCosts);
                        int maxAvailSoldiersByMP = house.manpower;
                        int numToRecruit = Mathf.Min(maxAvailSoldiersByGold, maxAvailSoldiersByMP);

                        goldPool.recruitmentGold -= numToRecruit * Soldiers.GetSoldierTypeStats(st).goldCosts;
                        house.manpower -= numToRecruit;
                        Soldiers soldiers = new Soldiers();
                        soldiers.SetSoldierTypeNum(st, numToRecruit);
                        AIGameActions.Recruit(gl, soldiers);

                        return;
                    }
                }
            }
        }
    }

    private void ManageBuilding() {
        List<GameLocation> castles = new List<GameLocation>();
        List<GameLocation> outposts = new List<GameLocation>();

        foreach (GameLocation gl in ownedLocations) {
            if (gl.GetType() == typeof(Castle)) {
                castles.Add(gl);
            } else {
                outposts.Add(gl);
            }
        }

        // Search castles that dont have a local administration and immediately build one
        foreach (GameLocation gl in castles) {
            if (!IsLocalAdministrationBuilt(gl)) {
                if (goldPool.buildingGold >= Building.GetBuildingTypeInfos(BuildingType.LOCAL_ADMINISTRATION).neededGold) BuildInLocation(gl, BuildingType.LOCAL_ADMINISTRATION);
                return;
            }
        }

        // Emergency build more gold buildings if manpower higher as gold (is bad balance)
        if (house.manpower > goldPool.recruitmentGold && goldPool.buildingGold >= Building.GetBuildingTypeInfos(BuildingType.MARKETPLACE).neededGold) {
            GameLocation buildLocation = SearchPlaceToBuild(BuildingType.MARKETPLACE, castles);
            if (buildLocation != null) {
                BuildInLocation(buildLocation, BuildingType.MARKETPLACE);
                return;
            }
        }

        // Search outposts in safe territory and build local admin if not built (only rarely)
        if (buildLocalAdminInSafeOutpostsCounter++ >= 10) {
            buildLocalAdminInSafeOutpostsCounter = 0;
            foreach (GameLocation gl in outposts) {
                if (!IsLocalAdministrationBuilt(gl)) {
                    bool isSafeOutpost = true;
                    foreach (GameLocation neighbour in gl.reachableLocations) {
                        if (neighbour.house.houseType != house.houseType) {
                            isSafeOutpost = false;
                            break;
                        }
                    }
                    if (isSafeOutpost) {
                        if (goldPool.buildingGold >= Building.GetBuildingTypeInfos(BuildingType.LOCAL_ADMINISTRATION).neededGold) BuildInLocation(gl, BuildingType.LOCAL_ADMINISTRATION);
                        return;
                    }
                }
            }
        }

        // Pick a random building with priorities
        int randMax = 0;
        foreach (var kv in buildingTypePriorities) {
            randMax += kv.Value;
        }
        int rand = Random.Range(0, randMax);

        int counter = 0;
        foreach (var kv in buildingTypePriorities) {
            counter += kv.Value;

            if (counter > rand) {
                if (goldPool.buildingGold >= Building.GetBuildingTypeInfos(kv.Key).neededGold) {
                    GameLocation buildLocation = SearchPlaceToBuild(kv.Key, ownedLocations);
                    if (buildLocation != null) {
                        BuildInLocation(buildLocation, kv.Key);
                        return;
                    }
                }
                break;
            }
        }
    }

    private bool IsLocalAdministrationBuilt(GameLocation gl) {
        foreach (Building b in gl.buildings) {
            if (b.buildingType == BuildingType.LOCAL_ADMINISTRATION) {
                return true;
            }
        }
        return false;
    }

    private void BuildInLocation(GameLocation gl, BuildingType bt) {
        // Reduce gold and build
        goldPool.buildingGold -= Building.GetBuildingTypeInfos(bt).neededGold;
        AIGameActions.Build(gl, bt);
    }

    private GameLocation SearchPlaceToBuild(BuildingType bt, List<GameLocation> gls) {
        if (house.buildableBuildings.Contains(bt)) {
            foreach (GameLocation gl in gls) {
                if (gl.buildableBuildings.Contains(bt) && gl.buildings.Find(b => b.buildingType == bt) == null) return gl;
            }
        }
        return null;
    }
}