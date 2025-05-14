using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon; // For future UI
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxCapacity = 5;
    private int currentFlowers = 0;

    // Called when trying to add a flower
    public bool TryAddFlower()
    {
        if (currentFlowers >= maxCapacity)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        currentFlowers++;
        Debug.Log($"Flower collected! ({currentFlowers}/{maxCapacity})");
        return true;
    }

    // Called when using/removing a flower
    public void RemoveFlower()
    {
        if (currentFlowers > 0)
        {
            currentFlowers--;
        }
    }

    // Helper property
    public bool IsFull => currentFlowers >= maxCapacity;
}