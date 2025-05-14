using UnityEngine;

public class QuestGiver : MonoBehaviour {
    public QuestSO quest;
    bool playerInRange;

    void Update() {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;

        // If quest not started, start it
        if (!QuestManager.Instance.activeQuests.Exists(q => q.questID == quest.questID)) {
            QuestManager.Instance.StartQuest(quest);
            Debug.Log("Started quest: " + quest.questTitle);
            return;
        }

        // If find step complete, complete talk step
        int talkIndex = quest.objectives.FindIndex(o => o.type == ObjectiveType.Talk);
        int progress = QuestManager.Instance.GetProgress(quest.questID, talkIndex);
        if (progress < quest.objectives[talkIndex].requiredAmount) {
            QuestManager.Instance.ReportProgress(quest.questID, talkIndex, 1);
            Debug.Log("Returned and talked to quest giver: " + quest.questTitle);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) playerInRange = true;
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}