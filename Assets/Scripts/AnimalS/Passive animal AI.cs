using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BunnyAI : MonoBehaviour
{
    public enum BunnyState { Idle, Wander, Wary, Flee }
    public BunnyState currentState;

    public float wanderRadius = 10f;
    public float idleTimeMin = 2f;
    public float idleTimeMax = 5f;
    public float awarenessRadius = 10f;
    public float fleeRadius = 5f;
    public float fleeSpeed = 6f;  // Speed when fleeing
    public float normalSpeed = 3.5f;  // Normal speed when wandering
    public float fleeDurationMin = 2f;
    public float fleeDurationMax = 4f;

    public Transform player;

    private NavMeshAgent agent;
    private bool isIdling;
    private bool isFleeing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Automatically find the player by tag
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player GameObject is tagged as 'Player'");
        }

        currentState = BunnyState.Idle;
        StartCoroutine(StateManager());
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Handle state transitions
        if (distanceToPlayer < fleeRadius && !isFleeing)
        {
            currentState = BunnyState.Flee;
        }
        else if (distanceToPlayer < awarenessRadius && !isFleeing)
        {
            currentState = BunnyState.Wary;
        }
        else if (!isIdling && !isFleeing)
        {
            currentState = BunnyState.Wander;
        }

        // Look at player if wary and standing still
        if (currentState == BunnyState.Wary && !agent.hasPath)
        {
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 2f);
        }
    }

    IEnumerator StateManager()
    {
        while (true)
        {
            switch (currentState)
            {
                case BunnyState.Idle:
                    isIdling = true;
                    agent.ResetPath();
                    float waitTime = Random.Range(idleTimeMin, idleTimeMax);
                    yield return new WaitForSeconds(waitTime);
                    isIdling = false;
                    currentState = BunnyState.Wander;
                    break;

                case BunnyState.Wander:
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                        agent.speed = normalSpeed;  // Normal speed when wandering
                        agent.SetDestination(newPos);
                        currentState = BunnyState.Idle;
                    }
                    break;

                case BunnyState.Wary:
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        currentState = BunnyState.Idle;
                    }
                    break;

                case BunnyState.Flee:
                    isFleeing = true;
                    agent.speed = fleeSpeed;  // Set speed to fleeSpeed when fleeing
                    Vector3 fleeDir = (transform.position - player.position).normalized;
                    Vector3 randomOffset = Random.insideUnitSphere * 5f;
                    randomOffset.y = 0;
                    Vector3 fleeTarget = transform.position + fleeDir * 10f + randomOffset;

                    agent.SetDestination(fleeTarget);

                    float fleeDuration = Random.Range(fleeDurationMin, fleeDurationMax);

                    // Keep checking if agent is too close to the target
                    float timeSpentFleeing = 0f;
                    while (timeSpentFleeing < fleeDuration)
                    {
                        if (!agent.pathPending && agent.remainingDistance < 1f)
                        {
                            // Reset flee destination if it's too close
                            fleeTarget = transform.position + fleeDir * 10f + randomOffset;
                            agent.SetDestination(fleeTarget);
                        }

                        timeSpentFleeing += Time.deltaTime;
                        yield return null;
                    }

                    isFleeing = false;
                    agent.speed = normalSpeed;  // Return to normal speed after fleeing
                    currentState = BunnyState.Idle;
                    break;
            }

            yield return null;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * distance;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, distance, layermask);

        return navHit.position;
    }
}
