using UnityEngine;

public class WaypointArrow : MonoBehaviour {
    private Transform target;

    /// <summary>
    /// Call this to point the arrow at the target transform.
    /// </summary>
    public void SetTarget(Transform t) {
        target = t;
    }

    void Update() {
        if (target == null) return;
        // Position the arrow a bit above the target
        transform.position = target.position + Vector3.up * 2f;
        // Always face the camera so it's readable
        var cam = Camera.main;
        if (cam != null) transform.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
    }
}