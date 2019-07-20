using UnityEngine;

public class SyncPosToGameObject : MonoBehaviour {
    public GameObject gameObjectToFollow;
    public bool x = false, y = false, z = false;

    private Vector3 newOwnPos;

    private void LateUpdate() {
        newOwnPos = this.gameObject.transform.localPosition;
        if (x) newOwnPos.x = gameObjectToFollow.transform.position.x;
        if (y) newOwnPos.y = gameObjectToFollow.transform.position.y;
        if (z) newOwnPos.z = gameObjectToFollow.transform.position.z;
        this.gameObject.transform.localPosition = newOwnPos;
    }
}