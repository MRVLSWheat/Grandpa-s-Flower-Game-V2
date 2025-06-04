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

    [Header("Player‐Distance Settings")]
    [Tooltip("How close the player must be (in world units) to press E and trigger dialogue")]
    public float speakRange = 3f;

    // Tracks when we last spoke
    private float _lastSpeakTime = -Mathf.Infinity;

    // For tracking trigger enter/exit
    private bool playerInRange = false;

    // Cached reference to the player's transform
    private Transform _playerT;

    void Awake() {
        // Make sure this NPC's collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Try to find the player once at Awake
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null) {
            _playerT = playerGO.transform;
        }
        else {
            Debug.LogWarning($"[NPCDialogue] No GameObject tagged 'Player' found in scene. Distance checks will not work.", this);
        }
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
        // Only allow "E to speak" if:
        //  1) The player was in trigger range (playerInRange == true), AND
        //  2) The player is still within speakRange distance, AND
        //  3) They press E, AND
        //  4) Cooldown has elapsed
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) {
            if (_playerT != null) {
                float dist = Vector3.Distance(_playerT.position, transform.position);
                if (dist <= speakRange) {
                    TrySpeak();
                }
                else {
                    // If the player somehow moved out of range without triggering OnTriggerExit,
                    // we force‐set playerInRange false so future E‐presses won't work until re‐entering.
                    playerInRange = false;
                }
            }
            else {
                // If we couldn't find the player Transform, fall back on the old behavior:
                TrySpeak();
            }
        }
    }

    private void TrySpeak() {
        // Another safety: if we have a valid player Transform, don't speak unless they're within speakRange
        if (_playerT != null) {
            float dist = Vector3.Distance(_playerT.position, transform.position);
            if (dist > speakRange) {
                return;
            }
        }

        // Check cooldown
        if (Time.time < _lastSpeakTime + speakCooldown) {
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

    // (Optional)–if you want to visualize speakRange in the editor:
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, speakRange);
    }
}
