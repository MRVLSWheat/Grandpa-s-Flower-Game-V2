using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the inventory UI visibility and updates for flower collection.
/// Requires proper setup in Inspector with all references assigned.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent panel containing all inventory UI elements")]
    [SerializeField] private GameObject inventoryPanel;

    [Tooltip("Text element displaying the flower count (TextMeshPro)")]
    [SerializeField] private TMP_Text flowerCountText;

    [Tooltip("Array of 5 Image components representing flower slots")]
    [SerializeField] private Image[] flowerIcons = new Image[5];

    // Reference to the PlayerInventory component
    private PlayerInventory inventory;

    // Tracks if inventory UI is currently visible
    private bool isVisible;

    /// <summary>
    /// Initial setup when the game starts
    /// </summary>
    void Start()
    {
        // Get reference to the inventory system
        inventory = GetComponent<PlayerInventory>();

        // Subscribe to inventory change events
        inventory.OnInventoryChanged += UpdateUI;

        // Ensure inventory starts hidden
        inventoryPanel.SetActive(false);
    }

    /// <summary>
    /// Handles input each frame
    /// </summary>
    void Update()
    {
        // Toggle inventory visibility when I key is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Shows/hides the inventory panel
    /// </summary>
    private void ToggleInventory()
    {
        // Flip visibility state
        isVisible = !isVisible;

        // Apply visibility to panel
        inventoryPanel.SetActive(isVisible);

        // Force immediate UI update
        UpdateUI();
    }

    /// <summary>
    /// Updates all UI elements to reflect current inventory state
    /// </summary>
    private void UpdateUI()
    {
        // Update the counter text (e.g., "FLOWERS: 3/5")
        flowerCountText.text = $"FLOWERS: {inventory.flowers.Count}/{inventory.maxCapacity}";

        // Update each icon's visibility based on inventory contents
        for (int i = 0; i < flowerIcons.Length; i++)
        {
            // Show icon if slot contains a flower, hide otherwise
            flowerIcons[i].enabled = i < inventory.flowers.Count;
        }
    }

    /// <summary>
    /// Clean up event subscriptions when destroyed
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        inventory.OnInventoryChanged -= UpdateUI;
    }
}