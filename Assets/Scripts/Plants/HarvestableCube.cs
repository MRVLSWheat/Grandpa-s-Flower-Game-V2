using UnityEngine;

// Handles flower harvesting interaction
public class HarvestableCube : MonoBehaviour
{
    [Header("Settings")]
    public GameObject flowerPrefab; // Reference to inventory item

    // Called when player harvests this flower
    public void Harvest(PlayerInventory inventory)
    {
        if (inventory.AddFlower(flowerPrefab))
        {
            Destroy(gameObject);
        }
    }
}