using UnityEngine;

/// <summary>
/// Handles flower harvesting and interaction with inventory
/// </summary>
public class FlowerHarvester : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How close player needs to be to harvest")]
    [SerializeField] private float harvestRange = 2f;

    // Reference to player's inventory
    private PlayerInventory inventory;

    void Start()
    {
        // Get reference to inventory (make sure it's on the same GameObject)
        inventory = GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError("PlayerInventory component missing!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryHarvestFlower();
        }
    }

    /// <summary>
    /// Attempts to harvest a nearby flower
    /// </summary>
    private void TryHarvestFlower()
    {
        // Check if inventory is full first
        if (inventory.IsFull)
        {
            Debug.Log("Inventory full - can't harvest more flowers!");
            return;
        }

        // Find nearby flowers
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, harvestRange);

        foreach (Collider collider in nearbyColliders)
        {
            // Check if object is harvestable
            if (collider.CompareTag("Flower"))
            {
                // Add to inventory
                if (inventory.AddFlower())
                {
                    // Only destroy if successfully added to inventory
                    Destroy(collider.gameObject);
                }
                return; // Exit after first successful harvest
            }
        }

        Debug.Log("No harvestable flowers nearby");
    }
}