// Assets/Scripts/FindTarget.cs
using UnityEngine;

public class FindTarget : MonoBehaviour {
    [Tooltip("Matches QuestSO.targetID")]
    public string targetID = "PersonNPC";
    [Tooltip("Optional: drag in your Quest Marker prefab here")]
    public GameObject marker;

    void Start() {
        // Auto-create placeholder if none assigned
        if (marker == null) {
            marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "MarkerPlaceholder";
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = Vector3.up * 2f;
            marker.transform.localScale = Vector3.one * 0.3f;
            var mr = marker.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));
            mr.material.color = Color.cyan;
            Destroy(marker.GetComponent<Collider>());
        }

        // Hide it initially...
        marker.SetActive(false);

        // Subscribe to quest state changes
        QuestManager.Instance.OnQuestsUpdated += RefreshMarker;

        // **NEW**: initialize marker visibility right away
        RefreshMarker();
    }

    void OnDestroy() {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestsUpdated -= RefreshMarker;
    }

    void RefreshMarker() {
        // Find the active quest that includes this "Find" objective
        var quest = QuestManager.Instance.activeQuests
            .Find(q => q.objectives.Exists(o => o.type == ObjectiveType.Find && o.targetID == targetID));
        if (quest == null) {
            marker.SetActive(false);
            return;
        }

        int idx  = quest.objectives.FindIndex(o => o.targetID == targetID);
        int prog = QuestManager.Instance.GetProgress(quest.questID, idx);
        int req  = quest.objectives[idx].requiredAmount;
        marker.SetActive(prog < req);
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        QuestManager.Instance.ReportFind(targetID);
        Debug.Log($"Found target: {targetID}");
        GetComponent<Collider>().enabled = false;
    }
}
