using UnityEngine;
using System.Collections.Generic;

// Handles all inventory storage and capacity logic
public class PlayerInventory : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum number of flowers player can carry")]
    public int maxCapacity = 5;

    [Header("Debug")]
    [Tooltip("Current list of collected flowers")]
    public List<GameObject> flowers = new List<GameObject>();

    // Event that triggers when inventory changes
    public System.Action OnInventoryChanged;

    // Main method to add flowers to inventory
    public bool AddFlower(GameObject flower)
    {
        if (IsFull())
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }

        flowers.Add(flower);
        Debug.Log($"Added flower. Total: {flowers.Count}/{maxCapacity}");
        OnInventoryChanged?.Invoke();
        return true;
    }

    // Removes the oldest flower from inventory
    public void RemoveFlower()
    {
        if (IsEmpty())
        {
            Debug.LogWarning("Inventory is empty!");
            return;
        }

        flowers.RemoveAt(0);
        Debug.Log($"Removed flower. Total: {flowers.Count}/{maxCapacity}");
        OnInventoryChanged?.Invoke();
    }

    // Helper methods
    public bool IsFull() => flowers.Count >= maxCapacity;
    public bool IsEmpty() => flowers.Count <= 0;
}