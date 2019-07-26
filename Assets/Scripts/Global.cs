using UnityEngine;

public static class Global {
    // Game Parameters
    public static HouseType GAME_PARAM_PLAYER_HOUSE_TYPE = HouseType.NEUTRAL;
    public static AIDifficulty GAME_PARAM_AI_DIFF = AIDifficulty.NORMAL;
    // --

    // Camera
    public static float CAMERA_DEFAULT_SPEED = 10f;
    public static float CAMERA_DEFAULT_ZOOM_SPEED = 50f;
    public static float CAMERA_SPEED = 10f;
    public static float CAMERA_ZOOM_SPEED = 50f;

    public static float CAMERA_BOUNDS_X_RIGHT = 12f;
    public static float CAMERA_BOUNDS_X_LEFT = -10f;

    public static float CAMERA_BOUNDS_Z_TOP = 26.3f;
    public static float CAMERA_BOUNDS_Z_BOTTOM = -24f;

    public static float CAMERA_BOUNDS_SCROLL_IN = 5f;
    public static float CAMERA_BOUNDS_SCROLL_OUT = 16f;
    // --

    // Game Controller
    public static float GAME_CONTROLLER_AI_HANDLE_CYCLE_TIME = 3.0f; // every cycle all AIs have played
    // --

    // Selection UI
    public static float SEL_UI_UPDATE_TIME = 0.1f;
    // --

    // Buildings UI
    public static float BUILD_UI_REFRESH_TIME = 1f;
    // --

    // Game Location
    public static float GAME_LOCATION_RESOURCES_UPDATE_TIME = 6f; // in seconds
    public static float GAME_LOCATION_GUI_UPDATE_TIME = 0.1f; // in seconds
    // --

    // Troops
    public static float TROOPS_BASE_MOVE_SPEED = 0.2f;
    public static float TROOPS_MOVE_SPEED_RAND_MIN = 0.9f; // Inclusive
    public static float TROOPS_MOVE_SPEED_RAND_MAX = 1.1f; // Inclusive
    // --

    // Combat
    public static float COMBAT_SPEED = 2f;
    public static float COMBAT_DAMAGE_RAND_MIN = 0.6f; // Inclusive
    public static float COMBAT_DAMAGE_RAND_MAX = 1.4f; // Inclusive
    public static float COMBAT_DAMAGE_DAMPER = 0.2f;
    // --

    // Paths
    public static string PATH_SETTINGS = Application.dataPath + "/settings.txt";
    // --

    public static void LimitCameraToBoundaries(Component camera) {
        Vector3 pos = camera.transform.position;
        pos.x = Mathf.Clamp(pos.x, CAMERA_BOUNDS_X_LEFT, CAMERA_BOUNDS_X_RIGHT);
        pos.y = Mathf.Clamp(pos.y, CAMERA_BOUNDS_SCROLL_IN, CAMERA_BOUNDS_SCROLL_OUT);
        pos.z = Mathf.Clamp(pos.z, CAMERA_BOUNDS_Z_BOTTOM, CAMERA_BOUNDS_Z_TOP);
        camera.transform.position = pos;
    }
}
