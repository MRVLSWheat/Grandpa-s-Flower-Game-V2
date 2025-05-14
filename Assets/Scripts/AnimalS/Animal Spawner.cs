using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The animal prefab to spawn (must have NavMeshAgent + your AI scripts).")]
    public GameObject animalPrefab;

    [Tooltip("How many to spawn at start.")]
    public int spawnCount = 10;

    [Tooltip("Radius around this object in which to spawn animals.")]
    public float spawnRadius = 20f;

    [Tooltip("How many attempts per animal to find a valid NavMesh spot.")]
    public int maxAttemptsPerSpawn = 10;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnOne();
    }

    /// <summary>
    /// Spawns a single animal somewhere on the NavMesh around this spawner.
    /// </summary>
    public void SpawnOne()
    {
        Vector3 spawnPos;
        if (TryFindNavMeshPoint(out spawnPos))
        {
            Instantiate(animalPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"[{name}] Could not find valid NavMesh point to spawn an animal.");
        }
    }

    /// <summary>
    /// Attempts to pick a random point within spawnRadius that lies on a NavMesh.
    /// </summary>
    private bool TryFindNavMeshPoint(out Vector3 result)
    {
        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            // random point in sphere around spawner
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            // sample within 2 units to snap to NavMesh
            if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
