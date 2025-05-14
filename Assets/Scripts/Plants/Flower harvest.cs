using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    [SerializeField] private float harvestRange = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryHarvest();
        }
    }

    private void TryHarvest()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, harvestRange);

        foreach (Collider col in nearbyObjects)
        {
            Harvestable flower = col.GetComponent<Harvestable>();
            if (flower != null)
            {

                flower.Harvest();
                return; // Stop after first harvest
            }
        }
    }
}