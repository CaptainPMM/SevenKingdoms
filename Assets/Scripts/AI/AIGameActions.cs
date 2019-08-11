public static class AIGameActions {
    /**
        Move all soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation, bool mpSend = true) {
        if (fromGameLocation.numSoldiers > 0) {
            Troops t = GameController.activeGameController.InitializeTroopsMovement(fromGameLocation.gameObject, toGameLocation.gameObject, fromGameLocation.soldiers, mpSend);
            fromGameLocation.soldiers = new Soldiers();
            AIPlayer.InformOfMovingTroops(t);
        }
    }

    /**
        Move a number of soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation, Soldiers soldiers, bool mpSend = true) {
        if (soldiers.GetNumSoldiersInTotal() > 0) {
            Soldiers soldiersToMove = new Soldiers();
            foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                soldiersToMove.SetSoldierType(st, fromGameLocation.soldiers.ExtractSoldiers(st, soldiers.GetSoldierTypeNum(st)));
            }

            Troops t = GameController.activeGameController.InitializeTroopsMovement(fromGameLocation.gameObject, toGameLocation.gameObject, soldiersToMove, mpSend);
            AIPlayer.InformOfMovingTroops(t);
        }
    }

    public static void Build(GameLocation gameLocation, BuildingType buildingType, bool mpSend = true) {
        gameLocation.AddBuilding(Building.CreateBuildingInstance(buildingType), mpSend);
    }

    public static void Recruit(GameLocation gameLocation, Soldiers soldiers, bool mpSend = true) {
        gameLocation.AddSoldiersToRecruitment(soldiers, mpSend);
    }
}