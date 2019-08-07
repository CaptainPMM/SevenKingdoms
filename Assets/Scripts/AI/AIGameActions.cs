public static class AIGameActions {
    /**
        Move all soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation) {
        if (fromGameLocation.numSoldiers > 0) {
            Troops t = GameController.activeGameController.InitializeTroopsMovement(fromGameLocation.gameObject, toGameLocation.gameObject, fromGameLocation.soldiers);
            fromGameLocation.soldiers = new Soldiers();
            AIPlayer.InformOfMovingTroops(t);

            if (Multiplayer.NetworkManager.isServer) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCMoveTroops(fromGameLocation, toGameLocation));
        }
    }

    /**
        Move a number of soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation, Soldiers soldiers) {
        if (soldiers.GetNumSoldiersInTotal() > 0) {
            Soldiers soldiersToMove = new Soldiers();
            foreach (SoldierType st in Soldiers.CreateSoldierTypesArray()) {
                soldiersToMove.SetSoldierType(st, fromGameLocation.soldiers.ExtractSoldiers(st, soldiers.GetSoldierTypeNum(st)));
            }

            Troops t = GameController.activeGameController.InitializeTroopsMovement(fromGameLocation.gameObject, toGameLocation.gameObject, soldiersToMove);
            AIPlayer.InformOfMovingTroops(t);

            if (Multiplayer.NetworkManager.isServer) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCMoveTroops(fromGameLocation, toGameLocation, soldiers));
        }
    }

    public static void Build(GameLocation gameLocation, BuildingType buildingType) {
        gameLocation.AddBuilding(Building.CreateBuildingInstance(buildingType));

        if (Multiplayer.NetworkManager.isServer) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCBuild(gameLocation, buildingType));
    }

    public static void Recruit(GameLocation gameLocation, Soldiers soldiers) {
        gameLocation.AddSoldiersToRecruitment(soldiers);

        if (Multiplayer.NetworkManager.isServer) Multiplayer.NetworkManager.Send(new Multiplayer.NetworkCommands.NCRecruit(gameLocation, soldiers));
    }
}