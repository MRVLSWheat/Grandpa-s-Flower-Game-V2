using UnityEngine;
using System.Collections.Generic;

namespace FlowerSpawner
{
    // Handles spawning and management of harvestable flower cubes in the game world
    public class Flowerspawning : MonoBehaviour
    {
        // The flower prefab to spawn (assign in Inspector)
        [SerializeField] private GameObject Flower;

        // Controls how many flowers exist in the scene (1-25)
        [SerializeField][Range(1, 25)] private int spawnCount = 10;

        // Parent objects for organization (assign in Inspector)
        [SerializeField] private GameObject TallFlowers;
        [SerializeField] private GameObject ShortFlowers;

        // Array tracking all active flower instances
        public GameObject[] flowers;

        // Current count of active flowers
        public int flowerCount;

        // Initialize first batch of flowers
        void Start()
        {
            SpawningFlowers();
        }

        // Main update loop - handles flower respawning
        void Update()
        {
            // Refresh list of active flowers
            flowers = GameObject.FindGameObjectsWithTag("Flower");
            flowerCount = flowers.Length;

            // Press 'I' to respawn flowers (day skip mechanic)
            if (Input.GetKeyDown("n"))
            {
                // Only respawn if between 3-9 flowers exist
                if (flowerCount < 10 && flowerCount > 2)
                {
                    SpawningFlowers();
                }
            }
        }

        // Spawns new flowers until reaching spawnCount
        void SpawningFlowers()
        {
            // Calculate how many flowers need to be spawned
            int flowersToSpawn = spawnCount - flowerCount;

            for (int i = 0; i < flowersToSpawn; i++)
            {
                // Create new flower at random position
                GameObject newFlower = Instantiate(Flower, GetRandomPosition(), Quaternion.identity);

                // Note: Size randomization is handled by HarvestableCube.Start()
                // Parent assignment is now handled by HarvestableCube
            }
        }

        // Generates a random spawn position within defined area
        private Vector3 GetRandomPosition()
        {
            return new Vector3(
                // Random X position within 10 units of spawner
                Random.Range(transform.position.x - 10, transform.position.x + 10),

                // Fixed Y position (9 units up)
                9,

                // Random Z position within 10 units of spawner
                Random.Range(transform.position.z - 10, transform.position.z + 10)
            );
        }
    }
}