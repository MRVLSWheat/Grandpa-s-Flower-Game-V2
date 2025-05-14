using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class QuestManager : MonoBehaviour {
    public static QuestManager Instance { get; private set; }

    public List<QuestSO> activeQuests = new List<QuestSO>();
    public List<string> completedQuests = new List<string>();
    private Dictionary<string, int[]> progress = new Dictionary<string, int[]>();
    public event Action OnQuestsUpdated;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void StartQuest(QuestSO quest) {
        if (completedQuests.Contains(quest.questID) || activeQuests.Contains(quest)) return;
        activeQuests.Add(quest);
        progress[quest.questID] = new int[quest.objectives.Count];
        OnQuestsUpdated?.Invoke();
    }

    public void ReportProgress(string questID, int objIndex, int amount = 1) {
        if (!progress.ContainsKey(questID)) return;
        var counts = progress[questID];
        var quest = activeQuests.First(q => q.questID == questID);
        counts[objIndex] = Mathf.Min(
            counts[objIndex] + amount,
            quest.objectives[objIndex].requiredAmount
        );
        OnQuestsUpdated?.Invoke();

        // Check full completion of all objectives
        bool allComplete = true;
        for (int i = 0; i < counts.Length; i++) {
            if (counts[i] < quest.objectives[i].requiredAmount) {
                allComplete = false;
                break;
            }
        }
        if (allComplete) {
            CompleteQuest(questID);
        }
    }

    // Handle "Find" objectives
    public void ReportFind(string targetID) {
        // Copy activeQuests to avoid modifying collection during iteration
        var questsToCheck = new List<QuestSO>(activeQuests);
        foreach (var quest in questsToCheck) {
            for (int i = 0; i < quest.objectives.Count; i++) {
                var obj = quest.objectives[i];
                if (obj.type == ObjectiveType.Find && obj.targetID == targetID) {
                    ReportProgress(quest.questID, i, 1);
                }
            }
        }
    }

    void CompleteQuest(string questID) {
        var quest = activeQuests.First(q => q.questID == questID);
        activeQuests.Remove(quest);
        completedQuests.Add(questID);
        progress.Remove(questID);
        OnQuestsUpdated?.Invoke();
        Debug.Log($"Quest complete: {quest.questTitle}");
    }

    public int GetProgress(string questID, int objIndex) {
        return progress.ContainsKey(questID) ? progress[questID][objIndex] : 0;
    }
}