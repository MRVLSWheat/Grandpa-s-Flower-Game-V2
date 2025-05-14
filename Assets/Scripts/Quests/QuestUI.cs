using UnityEngine;
using TMPro;
using System.Text;

public class QuestUI : MonoBehaviour {
    [Tooltip("Drag your TextMeshPro - Text component here")]
    public TMP_Text questText;

    void Start() {
        // Subscribe to quest updates
        QuestManager.Instance.OnQuestsUpdated += Refresh;
        Refresh();
    }

    void OnDestroy() {
        // Unsubscribe when destroyed
        QuestManager.Instance.OnQuestsUpdated -= Refresh;
    }

    void Refresh() {
        var sb = new StringBuilder();
        foreach (var q in QuestManager.Instance.activeQuests) {
            sb.AppendLine($"<b>{q.questTitle}</b>");
            for (int i = 0; i < q.objectives.Count; i++) {
                var obj = q.objectives[i];
                int prog = QuestManager.Instance.GetProgress(q.questID, i);
                sb.AppendLine($"  • {obj.description}: {prog}/{obj.requiredAmount}");
            }
        }
        // Update the TMP text field
        questText.text = sb.ToString();
    }
}