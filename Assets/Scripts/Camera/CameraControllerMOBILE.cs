using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControllerMOBILE : MonoBehaviour {
    public float scrollingSpeed;
    public float scrollHeightModifier = 0.1f;

    private Vector3 touchStart;

    private void Awake() {
        scrollingSpeed = Global.CAMERA_ZOOM_SPEED * 0.03f; // Adjust mobile zoom speed
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
            touchStart = GetWorldPosition();
        }

        // ZOOMING / SCROLLING
        if (Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (!EventSystem.current.IsPointerOverGameObject(touch0.fingerId) && !EventSystem.current.IsPointerOverGameObject(touch1.fingerId)) {
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

                // Also allow movement while zooming
                Vector3 direction = touchStart - GetWorldPosition();
                gameObject.transform.position += direction * 10f * CalculateScrollHeightModifier() * Time.deltaTime;

                Global.LimitCameraToBoundaries(this);
            }
        } else {
            // MOVING
            if (!GameController.activeGameController.dragging) {
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                    Vector3 direction = touchStart - GetWorldPosition();
                    gameObject.transform.position += direction;
                    Global.LimitCameraToBoundaries(this);
                }
            }
        }
    }

    float CalculateScrollHeightModifier() {
        return transform.position.y * scrollHeightModifier;
    }

    private Vector3 GetWorldPosition() {
        Ray touchPos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float distance;
        ground.Raycast(touchPos, out distance);
        return touchPos.GetPoint(distance);
    }
}
