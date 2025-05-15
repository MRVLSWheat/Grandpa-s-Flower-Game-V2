using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple inventory system that stores harvested flowers
/// Maximum capacity: 5 flowers
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // List to store our collected flowers
    private List<string> flowers = new List<string>();

    // Maximum inventory capacity
    private const int MAX_CAPACITY = 5;

    // Public property to check current flower count
    public int FlowerCount => flowers.Count;

    // Public property to check if inventory is full
    public bool IsFull => flowers.Count >= MAX_CAPACITY;

    /// <summary>
    /// Attempts to add a flower to inventory
    /// </summary>
    /// <returns>True if added successfully, false if inventory full</returns>
    public bool AddFlower()
    {
        if (IsFull)
        {
            Debug.Log("Inventory full! Can't collect more flowers.");
            return false;
        }

        flowers.Add("Flower");
        Debug.Log($"Added flower to inventory. Now have {flowers.Count}/{MAX_CAPACITY}");
        return true;
    }

    /// <summary>
    /// Removes one flower from inventory
    /// </summary>
    public void RemoveFlower()
    {
        if (flowers.Count > 0)
        {
            flowers.RemoveAt(flowers.Count - 1);
            Debug.Log($"Removed flower. Now have {flowers.Count}/{MAX_CAPACITY}");
        }
    }
}