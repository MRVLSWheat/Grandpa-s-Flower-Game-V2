using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public float radius = 10f; // Define the size of the spawn area
    public GameObject animalPrefab; // Assign an animal prefab
    public int maxAnimals = 10; // Limit the number of animals in this area

    private int currentAnimalCount;

    void Start()
    {
        SpawnAnimals();
    }

    void SpawnAnimals()
    {
        for (int i = 0; i < maxAnimals; i++)
        {
            Vector3 spawnPos = GetRandomPointInCircle();
            Instantiate(animalPrefab, spawnPos, Quaternion.identity);
            currentAnimalCount++;
        }
    }

    Vector3 GetRandomPointInCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);
    }
}
