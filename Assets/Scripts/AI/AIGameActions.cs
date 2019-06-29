using UnityEngine;

public static class AIGameActions {
    /**
        Move all soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation) {
        if (fromGameLocation.numSoldiers > 0) {
            GameController.activeGameController.InitializeTroopsMovement(fromGameLocation.gameObject, toGameLocation.gameObject, fromGameLocation.soldiers);
            fromGameLocation.soldiers = new Soldiers();
        }
    }

    /**
        Move a number of soldiers to a new location
     */
    public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation, Soldiers soldiers) {
        Debug.LogWarning("public static void MoveTroops(GameLocation fromGameLocation, GameLocation toGameLocation, Soldiers soldiers) is not implemented");
    }

    public static void Build(GameLocation gameLocation, BuildingType buildingType) {
        gameLocation.AddBuilding(Building.CreateBuildingInstance(buildingType));
    }

    public static void Recruit(GameLocation gameLocation, Soldiers soldiers) {
        gameLocation.AddSoldiersToRecruitment(soldiers);
    }
}