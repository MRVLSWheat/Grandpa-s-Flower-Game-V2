using UnityEngine;

/// Handles cube behavior including size randomization and harvesting.
/// Attach this to any object that should be spawnable/harvestable.
public class HarvestableCube : MonoBehaviour
{
    [Header("Size Settings")]
    [Tooltip("Minimum possible size (XYZ) when spawning")]
    public Vector3 minSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Tooltip("Maximum possible size (XYZ) when spawning")]
    public Vector3 maxSize = new Vector3(2f, 2f, 2f);

    [Header("Current Size")]
    [Tooltip("The actual size of this cube instance (read-only for debugging)")]
    public Vector3 currentSize;

    /// Called when the cube spawns. Randomizes its initial size.
    void Start()
    {
        RandomizeSize();
    }

    /// Generates a random size between minSize and maxSize for this cube
    public void RandomizeSize()
    {
        currentSize = new Vector3(
            Random.Range(minSize.x, maxSize.x),
            Random.Range(minSize.y, maxSize.y),
            Random.Range(minSize.z, maxSize.z)
        );
        transform.localScale = currentSize;
    }

    /// Called when player harvests this cube.
    /// Logs the size and destroys the object.
    public void Harvest()
    {
        Debug.Log($"Harvested cube of size: {currentSize}");
        Destroy(gameObject);
    }
}