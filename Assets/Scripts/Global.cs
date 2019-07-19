using UnityEngine;

public class Global {
    // Camera
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
    public static float SEL_UI_UPDATE_TIME = 0.3f;
    // --

    // Buildings UI
    public static float BUILD_UI_REFRESH_TIME = 2f;
    // --

    // Game Location
    public static float GAME_LOCATION_RESOURCES_UPDATE_TIME = 5f; // in seconds
    public static float GAME_LOCATION_GUI_UPDATE_TIME = 0.1f; // in seconds
    // --

    // Troops
    public static float TROOPS_BASE_MOVE_SPEED = 0.2f;
    public static float TROOPS_MOVE_SPEED_RAND_MIN = 0.9f; // Inclusive
    public static float TROOPS_MOVE_SPEED_RAND_MAX = 1.1f; // Inclusive
    // --

    // Combat
    public static float COMBAT_SPEED = 1.5f;
    public static float COMBAT_DAMAGE_RAND_MIN = 0.6f; // Inclusive
    public static float COMBAT_DAMAGE_RAND_MAX = 1.4f; // Inclusive
    public static float COMBAT_DAMAGE_DAMPER = 0.2f;
    // --

    public static void LimitCameraToBoundaries(Component camera) {
        Vector3 pos = camera.transform.position;
        pos.x = Mathf.Clamp(pos.x, CAMERA_BOUNDS_X_LEFT, CAMERA_BOUNDS_X_RIGHT);
        pos.y = Mathf.Clamp(pos.y, CAMERA_BOUNDS_SCROLL_IN, CAMERA_BOUNDS_SCROLL_OUT);
        pos.z = Mathf.Clamp(pos.z, CAMERA_BOUNDS_Z_BOTTOM, CAMERA_BOUNDS_Z_TOP);
        camera.transform.position = pos;
    }
}
