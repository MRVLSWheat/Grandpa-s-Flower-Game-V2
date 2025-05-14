using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    public GameObject spawnAreaPrefab;
    public int numberOfSpawnAreas = 3;

    private List<GameObject> spawnAreas = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < numberOfSpawnAreas; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            GameObject area = Instantiate(spawnAreaPrefab, randomPos, Quaternion.identity);
            spawnAreas.Add(area);
        }
    }
}
