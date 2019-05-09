using UnityEngine;

public class SelectionMarker : MonoBehaviour {
    [SerializeField]
    private float rotationSpeed = 55f;

    // Update is called once per frame
    void Update() {
        GetComponentInChildren<SpriteRenderer>().transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
