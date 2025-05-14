// Assets/Scripts/DialogueUI.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour {
    public static DialogueUI Instance { get; private set; }

    [Tooltip("The panel GameObject to show/hide")]
    public GameObject dialoguePanel;
    [Tooltip("The TMP_Text component inside that panel")]
    public TMP_Text dialogueText;
    [Tooltip("Seconds each line remains visible")]
    public float displayTime = 3f;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Call this to display a single line of dialogue.
    /// </summary>
    public void ShowLine(string line) {
        StopAllCoroutines();
        dialogueText.text = line;
        dialoguePanel.SetActive(true);
        StartCoroutine(HideAfter());
    }

    IEnumerator HideAfter() {
        yield return new WaitForSeconds(displayTime);
        dialoguePanel.SetActive(false);
    }
}
