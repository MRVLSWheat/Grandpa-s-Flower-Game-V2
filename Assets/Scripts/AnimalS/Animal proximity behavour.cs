using UnityEngine;
using UnityEngine.AI;

public class AnimalProximityBehaviour : MonoBehaviour
{
    public float firstStageDistance = 10f;
    public float secondStageDistance = 5f;
    public float minRunDistance = 5f;
    public float maxRunDistance = 15f;
    public float fleeAngleVariation = 45f;

    private Transform player;
    private Animator animator;
    private Renderer rend;
    private NavMeshAgent agent;

    private bool isLookingAtPlayer = false;
    private bool isRunningAway = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure your player has the 'Player' tag.");
        }

        SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral pink at start
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isRunningAway)
        {
            SetColor(Color.red); // Stage 2 (running)
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                StopRunningAway();
            }
        }
        else if (distanceToPlayer <= secondStageDistance)
        {
            StartRunningAway();
            SetColor(Color.red); // Stage 2 (starting to run)
        }
        else if (distanceToPlayer <= firstStageDistance)
        {
            LookAtPlayer();
            SetColor(Color.yellow); // Stage 1 (alert)
        }
        else
        {
            ResetBehavior();
            SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral (pink)
        }
    }

    private void LookAtPlayer()
    {
        if (!isLookingAtPlayer)
        {
            isLookingAtPlayer = true;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            agent.ResetPath(); // Stop any NavMesh movement
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void StartRunningAway()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        Vector3 awayDirection = (transform.position - player.position).normalized;
        float randomAngle = Random.Range(-fleeAngleVariation, fleeAngleVariation);
        awayDirection = Quaternion.Euler(0, randomAngle, 0) * awayDirection;

        float randomDistance = Random.Range(minRunDistance, maxRunDistance);
        Vector3 targetPosition = transform.position + awayDirection * randomDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, maxRunDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            isRunningAway = true;
        }
        else
        {
            Debug.LogWarning("Could not find valid NavMesh position to flee to.");
        }
    }

    private void StopRunningAway()
    {
        isRunningAway = false;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void ResetBehavior()
    {
        isLookingAtPlayer = false;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        if (!isRunningAway)
        {
            agent.ResetPath(); // Stop movement when not fleeing
        }
    }

    private void SetColor(Color color)
    {
        if (rend != null && rend.material.color != color)
        {
            rend.material.color = color;
        }
    }

    // Public getter for isRunningAway
    public bool IsRunningAway()
    {
        return isRunningAway;
    }
}
