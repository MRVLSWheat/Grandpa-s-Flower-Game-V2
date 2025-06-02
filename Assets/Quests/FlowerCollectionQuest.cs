using UnityEngine;
using UnityEngine.UI;    // for Canvas, Text, etc.

/// <summary>
/// A “collect flowers” quest that:
/// 1. Begins when the player interacts with a specified NPC (E in the NPC’s trigger).
/// 2. Spawns its own Canvas + three Text elements at runtime:
///    • Progress Text: shows “current/required flowers” (e.g. “3/10 flowers”).
///    • Start Text: shows a custom “quest start” message you set in the Inspector.
///    • Completion Text: shows a custom “quest complete” message you set in the Inspector.
///    All three can be independently positioned via Inspector‐exposed Vector2 fields.
/// 3. Requires the player to return to the NPC and hand in the flowers (press E again) to complete.
/// 
/// Usage:
/// • Make sure this GameObject also has your PlayerInventory component.
/// • In the Inspector:
///     • Drag your PlayerInventory into “Player Inventory”.
///     • Drag your NPC GameObject (with an isTrigger Collider) into “Quest Giver NPC”.
///     • Set “Required Flowers” to how many flowers must be collected (e.g. 10).
///     • Fill in “Start Message” and “Completion Message” with the strings you want displayed.
///     • Tweak each of these for positioning (in pixels, relative to top-left):
///         – Progress Text Anchored Position
///         – Start Text Anchored Position
///         – Completion Text Anchored Position
///     • Optionally adjust “Font Size” and “Font Color” for all three texts.
/// • Hit Play. Walk into the NPC’s trigger and press E → “Start Text” appears and Progress text appears.
///   As you collect flowers, the Progress text updates. Once you have enough, return and press E again → “Completion Text” appears.
/// </summary>
public class FlowerCollectionQuest : MonoBehaviour
{
    [Header("Quest Settings")]
    [Tooltip("How many flowers the player must collect to complete this quest.")]
    [SerializeField] private int requiredFlowers = 10;

    [Header("References")]
    [Tooltip("Drag in your PlayerInventory component (must be on this GameObject or assigned manually).")]
    [SerializeField] private PlayerInventory playerInventory;

    [Tooltip("Drag in the NPC GameObject that gives/turns-in this quest. Must have a Collider set to isTrigger.")]
    [SerializeField] private GameObject questGiverNpc;

    [Header("Start / Completion Messages")]
    [Tooltip("Text that will appear when the quest starts.")]
    [SerializeField] private string startMessage = "Quest Started! Go collect flowers.";

    [Tooltip("Text that will appear when the quest is completed.")]
    [SerializeField] private string completionMessage = "Thank you! Quest Complete.";

    [Header("Auto-Created UI Settings")]
    [Tooltip("Where to place the progress text (anchor = top-left).")]
    [SerializeField] private Vector2 progressTextAnchoredPosition = new Vector2(10, -10);

    [Tooltip("Where to place the ‘start’ text (anchor = top-left).")]
    [SerializeField] private Vector2 startTextAnchoredPosition = new Vector2(10, -40);

    [Tooltip("Where to place the ‘completion’ text (anchor = top-left).")]
    [SerializeField] private Vector2 completionTextAnchoredPosition = new Vector2(10, -70);

    [Tooltip("Font size for all UI texts.")]
    [SerializeField] private int fontSize = 24;

    [Tooltip("Font color for all UI texts.")]
    [SerializeField] private Color fontColor = Color.white;

    // --- Internal state flags ---
    private bool questActive = false;
    private bool questCompleted = false;
    private bool canInteractWithNpc = false;

    // References to the auto-created UI
    private Canvas    questCanvas;
    private Text      progressText;
    private Text      startText;
    private Text      completionText;

    private void Reset()
    {
        // If someone adds this script in the Editor, try to auto-find PlayerInventory on the same GameObject
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        // 1) Validate required references:
        if (playerInventory == null)
        {
            Debug.LogError($"[{nameof(FlowerCollectionQuest)}] No PlayerInventory assigned! Please set it in the Inspector.");
            enabled = false;
            return;
        }

        if (questGiverNpc == null)
        {
            Debug.LogError($"[{nameof(FlowerCollectionQuest)}] No Quest Giver NPC assigned! Please drag the NPC GameObject in the Inspector.");
            enabled = false;
            return;
        }

        // 2) Create our Canvas + Text elements at runtime:
        CreateProgressUI();

        Debug.Log($"[Quest] Approach the NPC and press E to start a {requiredFlowers}-flower quest.");
    }

    private void Update()
    {
        // If the quest is active but not yet completed, update the progress text each frame
        if (questActive && !questCompleted)
        {
            int currentCount = playerInventory.FlowerCount;
            progressText.text = $"{currentCount}/{requiredFlowers} flowers";
        }

        // If player is in range of the NPC and presses E:
        if (canInteractWithNpc && Input.GetKeyDown(KeyCode.E))
        {
            if (!questActive)
            {
                StartQuest();
            }
            else if (!questCompleted)
            {
                TryTurnInQuest();
            }
            // else: quest has already been completed → do nothing (or show a “thanks” prompt if desired)
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // We only care about entering the quest-giver NPC’s trigger
        if (other.gameObject == questGiverNpc)
        {
            canInteractWithNpc = true;
            Debug.Log("[Quest] Press E to talk to the NPC.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == questGiverNpc)
        {
            canInteractWithNpc = false;
            // (If you have an on-screen “Press E” prompt, hide it here.)
        }
    }

    /// <summary>
    /// Called once, when the player presses E on the NPC to start the quest.
    /// Shows the “start” text and progress text.
    /// </summary>
    private void StartQuest()
    {
        questActive = true;
        Debug.Log($"[Quest] Quest started! Collect {requiredFlowers} flowers and return to the NPC.");

        // Show the start text (with the inspector‐configured content)
        startText.text = startMessage;
        startText.gameObject.SetActive(true);

        // Show the progress text and initialize it to “0/required flowers”
        progressText.text = $"0/{requiredFlowers} flowers";
        progressText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the player presses E on the NPC again (while quest is active but not yet completed).
    /// If they have enough flowers, remove them, hide UI, mark complete, and invoke OnQuestCompleted().
    /// Otherwise, log how many more are needed.
    /// </summary>
    private void TryTurnInQuest()
    {
        int currentCount = playerInventory.FlowerCount;
        if (currentCount >= requiredFlowers)
        {
            // Remove exactly requiredFlowers from inventory
            for (int i = 0; i < requiredFlowers; i++)
            {
                playerInventory.RemoveFlower();
            }

            // Update any internal flowerAmount field (if you rely on that)
            playerInventory.FlowerAmount();

            questCompleted = true;
            Debug.Log($"[Quest] You handed in {requiredFlowers} flowers. Quest complete!");

            // Hide the start & progress texts; show the completion text
            startText.gameObject.SetActive(false);
            progressText.gameObject.SetActive(false);

            completionText.text = completionMessage;
            completionText.gameObject.SetActive(true);

            OnQuestCompleted();
        }
        else
        {
            int needed = requiredFlowers - currentCount;
            Debug.Log($"[Quest] You still need {needed} more flower{(needed > 1 ? "s" : "")}.");
        }
    }

    /// <summary>
    /// Override this in a subclass (or subscribe externally) to grant XP / items / trigger next quest, etc.
    /// </summary>
    protected virtual void OnQuestCompleted()
    {
        Debug.Log("[Quest] OnQuestCompleted() called. Override this method to give rewards or trigger the next quest.");
    }

    /// <summary>
    /// If you want to test or reuse the quest at runtime, call this to reset its state.
    /// Note: it does NOT refund flowers—if the player still has them, they could immediately turn in again.
    /// </summary>
    public void ResetQuest()
    {
        questActive = false;
        questCompleted = false;

        // Hide all three texts
        startText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        completionText.gameObject.SetActive(false);

        Debug.Log("[Quest] Quest has been reset. Talk to the NPC to start again.");
    }

    /// <summary>
    /// Creates a new Canvas (Screen Space – Overlay) and three child Text elements at runtime:
    /// • progressText
    /// • startText
    /// • completionText
    /// Each is anchored to top-left, and its anchoredPosition comes from the Inspector.
    /// </summary>
    private void CreateProgressUI()
    {
        // 1) Create a new Canvas GameObject
        GameObject canvasGO = new GameObject("QuestCanvas");
        canvasGO.transform.SetParent(this.transform, false);

        questCanvas = canvasGO.AddComponent<Canvas>();
        questCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 2) Create Progress Text
        progressText = CreateUIText(
            parent:      canvasGO.transform,
            name:        "QuestProgressText",
            anchoredPos: progressTextAnchoredPosition,
            initialText: "",
            sizeDelta:   new Vector2(300f, 40f) // width=300, height=40
        );
        progressText.gameObject.SetActive(false);

        // 3) Create Start Text
        startText = CreateUIText(
            parent:      canvasGO.transform,
            name:        "QuestStartText",
            anchoredPos: startTextAnchoredPosition,
            initialText: "",
            sizeDelta:   new Vector2(400f, 40f) // width=400, height=40
        );
        startText.gameObject.SetActive(false);

        // 4) Create Completion Text
        completionText = CreateUIText(
            parent:      canvasGO.transform,
            name:        "QuestCompletionText",
            anchoredPos: completionTextAnchoredPosition,
            initialText: "",
            sizeDelta:   new Vector2(400f, 40f) // width=400, height=40
        );
        completionText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Helper method: 
    /// Creates a Text under the given parent, anchored top-left, with the specified anchoredPosition & size.
    /// Uses LegacyRuntime.ttf as the built-in font.
    /// </summary>
    private Text CreateUIText(Transform parent, string name, Vector2 anchoredPos, string initialText, Vector2 sizeDelta)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);

        Text txt = textGO.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = fontSize;
        txt.color = fontColor;
        txt.alignment = TextAnchor.UpperLeft;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.text = initialText;

        RectTransform rt = txt.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);    // top-left corner
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        return txt;
    }
}
