using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5f;
    public float interactionRange = 2f;

    void Update()
    {
        // Movement
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;

        // Harvest input
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryHarvestCube();
        }
    }

    private void TryHarvestCube()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider collider in hitColliders)
        {
            HarvestableCube cube = collider.GetComponent<HarvestableCube>();
            if (cube != null)
            {
                cube.Harvest();
                return; // Exit after first valid harvest
            }
        }
    }
}