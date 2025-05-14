using System;
using UnityEngine;
using UnityEngine.AI;
// alias to avoid ambiguity with System.Random
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalMovement : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderRadius        = 10f;
    public float wanderInterval      = 5f;

    [Header("Flee Settings")]
    public float detectionRadius     = 5f;
    public float safeDistance        = 15f;
    public float normalSpeed         = 3.5f;
    public float fleeSpeed           = 6f;

    [Header("Disturbance Tick")]
    [Tooltip("Seconds between disturbance‐ticks while fleeing")]
    public float disturbanceTickInterval = 1f;

    private NavMeshAgent agent;
    private Transform player;
    private float wanderTimer;

    // Fired once when fleeing starts
    public event Action OnStartFlee;
    // Fired every disturbanceTickInterval seconds while fleeing
    public event Action OnFleeTick;

    private enum State { Wandering, Fleeing }
    private State currentState = State.Wandering;

    private float disturbanceTickTimer;

    void Start()
    {
        agent       = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        player      = GameObject.FindGameObjectWithTag("Player").transform;
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == State.Wandering)
        {
            if (distToPlayer <= detectionRadius)
                BeginFlee();
            else
                Wander();
        }
        else // Fleeing
        {
            ContinueFlee();

            // fire tick events
            disturbanceTickTimer += Time.deltaTime;
            if (disturbanceTickTimer >= disturbanceTickInterval)
            {
                OnFleeTick?.Invoke();
                disturbanceTickTimer = 0f;
            }

            if (distToPlayer >= safeDistance)
                StopFlee();
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || agent.remainingDistance < 0.5f)
        {
            Vector3 dest;
            if (RandomNavMeshPoint(transform.position, wanderRadius, out dest))
                agent.SetDestination(dest);
            wanderTimer = wanderInterval;
        }
    }

    private void BeginFlee()
    {
        currentState            = State.Fleeing;
        agent.speed             = fleeSpeed;
        disturbanceTickTimer    = 0f;              // reset tick timer
        OnStartFlee?.Invoke();                    // initial bump if you want

        ContinueFlee();                            // first immediate set
    }

    private void ContinueFlee()
    {
        Vector3 fleeDir   = (transform.position - player.position).normalized;
        Vector3 rawTarget = transform.position + fleeDir * safeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawTarget, out hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(rawTarget);
    }

    private void StopFlee()
    {
        currentState         = State.Wandering;
        agent.speed          = normalSpeed;
        wanderTimer          = 0f;               // force new wander point
        disturbanceTickTimer = 0f;               // reset so we don’t leak
    }

    private bool RandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 rnd = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(rnd, out hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = center;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
