using UnityEngine;

public class CasualtiesPopup : MonoBehaviour {
    private float animationDuration;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start() {
        animationDuration = GetComponentInChildren<Animation>().clip.length;
        elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= animationDuration) {
            Destroy(this.gameObject);
        }
    }
}
