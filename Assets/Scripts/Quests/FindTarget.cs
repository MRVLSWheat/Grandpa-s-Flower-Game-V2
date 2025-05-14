using UnityEngine;

public class FindTarget : MonoBehaviour {
    [Tooltip("Matches QuestSO.targetID")]
    public string targetID = "PersonNPC";

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        QuestManager.Instance.ReportFind(targetID);
        GetComponent<Collider>().enabled = false;  // fire only once
        Debug.Log($"Found target: {targetID}");
    }
}