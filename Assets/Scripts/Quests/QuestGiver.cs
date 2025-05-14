// Assets/Scripts/QuestGiver.cs
using UnityEngine;

public class QuestGiver : MonoBehaviour {
    [Header("Quest Settings")]
    public QuestSO quest;             
    [Tooltip("Optional: drag in your Quest Marker prefab here")]
    public GameObject marker;

    bool playerInRange = false;

    void Start() {
        // Auto-create placeholder sphere if no marker prefab assigned
        if (marker == null) {
            marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "MarkerPlaceholder";
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = Vector3.up * 2f;
            marker.transform.localScale = Vector3.one * 0.3f;
            var mr = marker.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));
            mr.material.color = Color.yellow;
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
        bool completed = QuestManager.Instance.completedQuests.Contains(quest.questID);
        bool active    = QuestManager.Instance.activeQuests.Contains(quest);

        if (!active && !completed) {
            // Show marker to start quest
            marker.SetActive(true);
        }
        else if (active) {
            // Only show once the “Find” step is done, to hand in
            int findIdx = quest.objectives.FindIndex(o => o.type == ObjectiveType.Find);
            int done    = QuestManager.Instance.GetProgress(quest.questID, findIdx);
            int req     = quest.objectives[findIdx].requiredAmount;
            marker.SetActive(done >= req);
        }
        else {
            // Quest completed → hide
            marker.SetActive(false);
        }
    }

    void Update() {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;

        // Already completed?
        if (QuestManager.Instance.completedQuests.Contains(quest.questID)) {
            Debug.Log("You have already completed this quest.");
            return;
        }

        // Not active yet → start it
        if (!QuestManager.Instance.activeQuests.Contains(quest)) {
            QuestManager.Instance.StartQuest(quest);
            Debug.Log("Started quest: " + quest.questTitle);
            return;
        }

        // Active: check “Find” progress
        int findIdx  = quest.objectives.FindIndex(o => o.type == ObjectiveType.Find);
        int findProg = QuestManager.Instance.GetProgress(quest.questID, findIdx);
        int findReq  = quest.objectives[findIdx].requiredAmount;
        if (findProg < findReq) {
            Debug.Log("You haven’t found them yet!");
            return;
        }

        // Handle “Talk” objective to hand in
        int talkIdx  = quest.objectives.FindIndex(o => o.type == ObjectiveType.Talk);
        int talkProg = QuestManager.Instance.GetProgress(quest.questID, talkIdx);
        int talkReq  = quest.objectives[talkIdx].requiredAmount;
        if (talkProg < talkReq) {
            QuestManager.Instance.ReportProgress(quest.questID, talkIdx, 1);
            Debug.Log("You talked to the quest giver to complete the quest.");
        } else {
            Debug.Log("Quest is already in progress.");
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) playerInRange = true;
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
