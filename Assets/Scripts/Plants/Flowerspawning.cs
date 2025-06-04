using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;
using System.Collections;
using UnityEngine.Serialization;

namespace FlowerSpawner
{
    public class Flowerspawning : MonoBehaviour
    {
        [SerializeField] private GameObject Flower;
        [SerializeField][Range(1, 25)] private int spawnCount = 10;
        [SerializeField] private float minimalXrange = 10f;
        [SerializeField] private float maximalXrange = 10f;
        [SerializeField] private float minimalYrange = 10f;
        [SerializeField] private float maximalYrange = 10f;
        [SerializeField] private float heightSpawn = 0f;
        private int timer = 0;
        private bool resetTimer = false;
        [SerializeField] private Hidetheplane Hidetheplane;
        [SerializeField] private GameObject TallFlowers;
        [SerializeField] private GameObject ShortFlowers;
        [SerializeField] private List<GameObject> flowers = new List<GameObject>();
        public int flowerCount;

        void Start()
        {
            SpawningFlowers();
        }

        void Update()
        {
            flowerCount = flowers.Count;

            if (Input.GetKeyDown("n"))
            {
                Debug.Log("Flower count: " + flowers.Count + ". Spawning more flowers.");
            }

            if (flowers.Count < 8)
            {
                Debug.Log("Flower count is low: " + flowers.Count + ". Spawning more flowers.");
                StartCoroutine(Hidetheplane.ActivatePlantAlert());
            }
             



            if (flowers.Count < spawnCount)
            {
                timer++;
                if (resetTimer == true)
                {
                    timer = 0;
                    resetTimer = false;
                }
                if (timer > 1800 && flowers.Count >= 5) // Adjust this value to control respawn frequency
                {
                    SpawningFlowers();
                    resetTimer = true; // Reset timer after spawning
                }
            }
            else
            {
                resetTimer = true;
            }
        }

        void SpawningFlowers()
        {
            // Calculate how many flowers need to be spawned
            int flowersToSpawn = spawnCount - flowers.Count;

            for (int i = 0; i < flowersToSpawn; i++)
            {
                // Create new flower at random position
                GameObject newFlower = Instantiate(Flower, GetRandomPosition(), Quaternion.identity);
                flowers.Add(newFlower); 
            }
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomXZ = new Vector3(
                Random.Range(transform.position.x - minimalXrange, transform.position.x + maximalXrange),
                transform.position.y, // Start at spawner's Y
                Random.Range(transform.position.z - minimalYrange, transform.position.z + maximalYrange)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomXZ, out hit, 20f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            else
            {
                return new Vector3(randomXZ.x, 9, randomXZ.z);
            }
        }

        public void RemoveFlower(GameObject flower)
        {
            if (flowers.Contains(flower))
            {
                flowers.Remove(flower);
                Debug.Log("Flower removed. Remaining count: " + flowers.Count);
            }
            else
            {
                Debug.LogWarning("Attempted to remove a flower that does not exist in the list.");
            }
        }
    }
}