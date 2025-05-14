using UnityEngine;
using UnityEngine.AI;
// ensure Random refers to UnityEngine.Random
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class PredatorAI : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderRadius    = 15f;
    public float wanderInterval  = 3f;

    [Header("Chase Settings")]
    public float detectionRadius = 10f;
    public float chaseSpeed      = 7f;
    public float normalSpeed     = 3.5f;
    public float giveUpDistance  = 20f;

    [Header("Attack Settings")]
    public float attackRange     = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private float wanderTimer;
    private bool isChasing = false;

    void Start()
    {
        agent       = GetComponent<NavMeshAgent>();
        player      = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = normalSpeed;
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            if (distToPlayer <= detectionRadius)
                StartChase();
            else
                Wander();
        }
        else
        {
            // keep pursuing the player
            agent.SetDestination(player.position);

            if (distToPlayer <= attackRange)
                Attack();
            else if (distToPlayer > giveUpDistance)
                StopChase();
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

    private void StartChase()
    {
        isChasing   = true;
        agent.speed = chaseSpeed;
    }

    private void StopChase()
    {
        isChasing   = false;
        agent.speed = normalSpeed;
        wanderTimer = 0f; // force new wander point next update
    }

    private void Attack()
    {
        // TODO: replace with real damage/animation logic
        Debug.Log($"{name} attacks the player!");
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, giveUpDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
