using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float waitTime = 3f;
    public float wanderSpeed = 2f; // Normal wandering speed
    public float fleeSpeed = 5f;   // Faster fleeing speed

    private NavMeshAgent agent;
    private Vector3 centerPoint;
    private float timer;

    // Reference to AnimalProximityBehaviour for checking if animal is fleeing
    private AnimalProximityBehaviour proximityBehaviour;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        proximityBehaviour = GetComponent<AnimalProximityBehaviour>(); // Assuming both scripts are on the same GameObject
        centerPoint = transform.position; // Set initial spawn point as center
        agent.speed = wanderSpeed; // Set initial speed to wander speed
        PickNewDestination();
    }

    void Update()
    {
        if (proximityBehaviour != null && proximityBehaviour.IsRunningAway())
        {
            // If fleeing, set speed to fleeSpeed
            agent.speed = fleeSpeed;
        }
        else
        {
            // Otherwise, revert to wandering speed
            agent.speed = wanderSpeed;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                PickNewDestination();
                timer = 0;
            }
        }
    }

    void PickNewDestination()
    {
        Vector3 newPos = GetRandomPointInCircle();
        agent.SetDestination(newPos);
    }

    Vector3 GetRandomPointInCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        Vector3 finalPosition = new Vector3(centerPoint.x + randomPoint.x, transform.position.y, centerPoint.z + randomPoint.y);
        return finalPosition;
    }
}
