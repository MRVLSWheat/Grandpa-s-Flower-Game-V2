// Assets/Scripts/NPCDialogue.cs
using UnityEngine;

[System.Serializable]
public struct DisturbanceLine {
    public float minDisturbance;   // inclusive
    public float maxDisturbance;   // inclusive
    [TextArea] public string line;
}

[RequireComponent(typeof(Collider))]
public class NPCDialogue : MonoBehaviour {
    [Tooltip("Define what this NPC says at different disturbance ranges")]
    public DisturbanceLine[] lines;

    [Header("Cooldown Settings")]
    [Tooltip("Time in seconds before this NPC can speak again")]
    public float speakCooldown = 5f;

    // Tracks when we last spoke
    private float _lastSpeakTime = -Mathf.Infinity;
    private bool playerInRange = false;

    void Awake() {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        TrySpeak();
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    void Update() {
        // Optional: re-speak on E when in range, respecting cooldown
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) {
            TrySpeak();
        }
    }

    private void TrySpeak() {
        // Check cooldown
        if (Time.time < _lastSpeakTime + speakCooldown) {
            // still cooling down
            return;
        }
        Speak();
        _lastSpeakTime = Time.time;
    }

    private void Speak() {
        // Safeguard: ensure the DialogueUI exists
        if (DialogueUI.Instance == null) {
            Debug.LogError("[NPCDialogue] No DialogueUI instance found in scene!", this);
            return;
        }
        // Safeguard: ensure DisturbanceManager is present
        if (DisturbanceManager.Instance == null) {
            Debug.LogError("[NPCDialogue] No DisturbanceManager instance found in scene!", this);
            return;
        }

        float d = DisturbanceManager.Instance.disturbanceValue;
        // Find the first line whose range contains d
        foreach (var dl in lines) {
            if (d >= dl.minDisturbance && d <= dl.maxDisturbance) {
                DialogueUI.Instance.ShowLine(dl.line);
                Debug.Log($"[NPCDialogue] {name} says: {dl.line}");
                return;
            }
        }
        // Fallback
        DialogueUI.Instance.ShowLine("…");
    }
}
