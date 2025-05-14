using UnityEngine;

// Handles player harvesting input
public class PlayerHarvest : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Harvesting range in meters")]
    public float harvestRange = 2f;

    private PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryHarvest();
        }
    }

    private void TryHarvest()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, harvestRange))
        {
            HarvestableCube flower = hit.collider.GetComponent<HarvestableCube>();
            flower?.Harvest(inventory);
        }
    }
}