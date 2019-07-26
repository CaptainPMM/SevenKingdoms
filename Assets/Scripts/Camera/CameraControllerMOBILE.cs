using UnityEngine;

public class CameraControllerMOBILE : MonoBehaviour {
    public float cameraSpeed;
    public float scrollingSpeed;
    public float scrollHeightModifier = 0.1f;

    private bool dragging = false;

    private void Awake() {
        cameraSpeed = Global.CAMERA_SPEED;
        scrollingSpeed = Global.CAMERA_ZOOM_SPEED;
    }

    // Update is called once per frame
    void Update() {
        // MOVING
        if (Input.touchCount == 1) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                Ray ray = Camera.main.ScreenPointToRay(t.position);
                RaycastHit hit;

                // Check for hit valid game objects
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider.gameObject.tag == "background") {
                        dragging = true;
                    }
                }
            }

            if (dragging && t.phase == TouchPhase.Moved) {
                Vector2 movement = t.position - t.deltaPosition;

                transform.position += new Vector3(movement.x, 0, movement.y) * cameraSpeed * CalculateScrollHeightModifier() * Time.deltaTime;

                Global.LimitCameraToBoundaries(this);
            }

            if (dragging && t.phase == TouchPhase.Ended) dragging = false;
        }

        // ZOOMING / SCROLLING
        if (Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Find the magnitude/length of the vector (distance) between the touches in each frame
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Find the difference in the distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Now its time to zoom/scroll the camera
            transform.position += new Vector3(0, deltaMagnitudeDiff, 0) * scrollingSpeed * CalculateScrollHeightModifier() * Time.deltaTime;
            Global.LimitCameraToBoundaries(this);
        }
    }

    float CalculateScrollHeightModifier() {
        return transform.position.y * scrollHeightModifier;
    }
}
