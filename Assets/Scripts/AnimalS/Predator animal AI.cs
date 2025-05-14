using UnityEngine;
using UnityEngine.AI;

public class AnimalBehavior : MonoBehaviour
{
    public float wanderRadius = 10f; // Radius around spawn point
    public float wanderInterval = 5f; // Time before choosing a new destination
    public float chaseRadius = 15f; // Distance at which the animal starts chasing
    public float stopChaseRadius = 20f; // Distance at which the animal stops chasing

    private NavMeshAgent agent;
    private Vector3 spawnPosition;
    private bool isChasingPlayer = false;
    private Transform player;
    private Renderer objectRenderer;
    private Color defaultColor;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;

        // Find the player using the tag "Player" when the animal spawns
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure your player has the 'Player' tag.");
        }

        // Get the Renderer for color changes
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            defaultColor = objectRenderer.material.color;
        }

        // Start the wandering process
        InvokeRepeating("SetRandomDestination", 0, wanderInterval);
    }

    void Update()
    {
        if (player == null) return;

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within the chase radius and the predator isn't already chasing
        if (distanceToPlayer < chaseRadius && !isChasingPlayer)
        {
            StartChasing();
        }
        // If the player is out of the stop chase radius and the predator is chasing
        else if (distanceToPlayer > stopChaseRadius && isChasingPlayer)
        {
            StopChasing();
        }

        // If chasing, keep setting the destination to the player's position
        if (isChasingPlayer)
        {
            agent.SetDestination(player.position);
        }
        // If not chasing, wander to a random point
        else if (!isChasingPlayer && !agent.hasPath)
        {
            SetRandomDestination();
        }
    }

    void StartChasing()
    {
        isChasingPlayer = true;
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.red;
        }
    }

    void StopChasing()
    {
        isChasingPlayer = false;
        if (objectRenderer != null)
        {
            objectRenderer.material.color = defaultColor;
        }
        SetRandomDestination();
    }

    void SetRandomDestination()
    {
        // Get a random point within the wander radius
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        Vector3 targetPosition = spawnPosition + randomDirection;
        NavMeshHit hit;

        // Ensure the random point is on the NavMesh
        if (NavMesh.SamplePosition(targetPosition, out hit, wanderRadius, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }

        // Set the random destination for the NavMesh agent
        agent.SetDestination(targetPosition);
    }
}
