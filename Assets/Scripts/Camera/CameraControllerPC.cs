using UnityEngine;

public class CameraControllerPC : MonoBehaviour {
    public float cameraSpeed = 10f;
    public float scrollingSpeed = 50f;
    public float scrollHeightModifier = 0.1f;

    // Update is called once per frame
    void Update() {
        // Moving
        if (Input.anyKey) {
            float x = 0f;
            float z = 0f;

            if (Input.GetKey(KeyCode.LeftArrow)) x -= cameraSpeed;
            if (Input.GetKey(KeyCode.RightArrow)) x += cameraSpeed;
            if (Input.GetKey(KeyCode.UpArrow)) z += cameraSpeed;
            if (Input.GetKey(KeyCode.DownArrow)) z -= cameraSpeed;

            transform.position += new Vector3(x, 0, z) * CalculateScrollHeightModifier() * Time.deltaTime;

            Global.LimitCameraToBoundaries(this);
        }

        // Scrolling
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAxis != 0) {
            transform.position += -(new Vector3(0, scrollAxis, 0) * scrollingSpeed * CalculateScrollHeightModifier() * Time.deltaTime);
            Global.LimitCameraToBoundaries(this);
        }
    }

    float CalculateScrollHeightModifier() {
        return transform.position.y * scrollHeightModifier;
    }
}
