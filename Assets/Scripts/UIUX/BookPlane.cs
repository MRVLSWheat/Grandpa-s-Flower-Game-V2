using Unity.VisualScripting;
using UnityEngine;

public class BookPlane : MonoBehaviour
{
    public GameObject planePrefab;  // assign your prefab in the Inspector
    public GameObject PrefabTwo;
    public Transform spawnPoint;    // optional: where to spawn it
    public bool bookopened = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && bookopened == false)
        {
           
            SpawnPlane();
            bookopened = true;
        }
        else if (Input.GetKeyDown(KeyCode.B) && bookopened == true)
        {
            DeletePlane();
            bookopened = false;
        }
    }

    void SpawnPlane()
    {
        Vector3 position = spawnPoint ? spawnPoint.position : Vector3.zero;
        PrefabTwo = Instantiate(planePrefab, position, Quaternion.identity);
    }
    void DeletePlane()
    {
            Destroy(PrefabTwo);
    }
}
