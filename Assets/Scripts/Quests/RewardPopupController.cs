// Assets/Scripts/RewardPopupController.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class RewardPopupController : MonoBehaviour {
    [Header("Assign in Inspector")]
    [Tooltip("Prefab of a TextMeshProUGUI text-only object")]
    public GameObject rewardTextPrefab;
    [Tooltip("Canvas to parent the text under")]
    public Canvas uiCanvas;
    [Tooltip("How long the text stays visible")]
    public float displayDuration = 2f;

    [Header("Message Template")]
    [Tooltip("Use {0} as placeholder for the quest title")]
    public string messageTemplate = "🎉 Quest “{0}” complete!";

    void Start() {
        // Validate references
        if (rewardTextPrefab == null) {
            Debug.LogError("[RewardPopup] rewardTextPrefab is NOT assigned!", this);
            enabled = false; 
            return;
        }
        if (uiCanvas == null) {
            Debug.LogError("[RewardPopup] uiCanvas is NOT assigned!", this);
            enabled = false;
            return;
        }
        if (QuestManager.Instance == null) {
            Debug.LogError("[RewardPopup] No QuestManager.Instance found in scene!", this);
            enabled = false;
            return;
        }

        // Subscribe to quest completion
        QuestManager.Instance.OnQuestComplete += ShowReward;
    }

    void OnDestroy() {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestComplete -= ShowReward;
    }

    void ShowReward(QuestSO quest) {
        Debug.Log($"[RewardPopup] ShowReward called for quest: {quest.questTitle}");

        // Instantiate the text-only prefab under the Canvas
        GameObject txtGO = Instantiate(rewardTextPrefab, uiCanvas.transform);
        TMP_Text txt = txtGO.GetComponent<TMP_Text>();
        if (txt == null) {
            Debug.LogError("[RewardPopup] No TMP_Text found on prefab!", txtGO);
            Destroy(txtGO);
            return;
        }

        // Set the dynamic message using the template
        txt.text = string.Format(messageTemplate, quest.questTitle);

        // Destroy after delay
        StartCoroutine(DestroyAfter(txtGO, displayDuration));
    }

    IEnumerator DestroyAfter(GameObject go, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }
}
