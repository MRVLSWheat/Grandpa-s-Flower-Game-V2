using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace FlowerSpawner
{
    // Handles spawning and management of harvestable flower cubes in the game world
    public class Flowerspawning : MonoBehaviour
    {
        // The flower prefab to spawn (assign in Inspector)
        [SerializeField] private GameObject Flower;

        // Controls how many flowers exist in the scene (1-25)
        [SerializeField][Range(1, 25)] private int spawnCount = 10;
        [SerializeField] private float minimalXrange = 10f;
        [SerializeField] private float maximalXrange = 10f;
        [SerializeField] private float minimalYrange = 10f;
        [SerializeField] private float maximalYrange = 10f;
        [SerializeField] private float heightSpawn = 0f;
        private int timer = 0;
        private bool resetTimer = false;
        [SerializeField] private Hidetheplane Hidetheplane;

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

            if (flowerCount < 8)
            {
                Debug.Log("Flower count is low: " + flowerCount + ". Spawning more flowers.");
                StartCoroutine(Hidetheplane.ActivatePlantAlert());
            }

            if (Input.GetKeyDown("n"))
            {
                // Debugging: Log current flower count
                Debug.Log("Current flower count: " + flowerCount);
            }



            if (flowerCount < spawnCount)
            {
                // If flower count is below target, start respawning
                if (resetTimer == true)
                {
                    timer = 0;
                    resetTimer = false;
                }
                timer++;
                if (timer > 60 && flowerCount >= 5) // Adjust this value to control respawn frequency
                {
                    SpawningFlowers();
                    resetTimer = true; // Reset timer after spawning
                }
            }
            else
            {
                // Reset timer if we have enough flowers
                resetTimer = true;
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
            // Generate a random XZ position within 10 units of the spawner
            Vector3 randomXZ = new Vector3(
                Random.Range(transform.position.x - minimalXrange, transform.position.x + maximalXrange),
                transform.position.y, // Start at spawner's Y
                Random.Range(transform.position.z - minimalYrange, transform.position.z + maximalYrange)
            );

            NavMeshHit hit;
            // Sample the NavMesh at the random XZ position, within a max distance (e.g., 20 units)
            if (NavMesh.SamplePosition(randomXZ, out hit, 20f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            else
            {
                // Fallback: return the original random position at fixed height if NavMesh not found
                return new Vector3(randomXZ.x, 9, randomXZ.z);
            }
        }
    }
}