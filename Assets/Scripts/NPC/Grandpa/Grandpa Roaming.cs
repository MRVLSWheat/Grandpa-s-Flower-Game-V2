using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCRandomRoam : MonoBehaviour
{
    [Header("Roaming Settings")]
    [Tooltip("Object to roam around. If null, uses NPC's start position.")]
    public Transform roamCenter;
    [Tooltip("Radius around the center point in which the NPC will wander.")]
    public float roamRadius = 10f;
    [Tooltip("Time in seconds to wait once reaching each random point.")]
    public float waitTime = 3f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Roam());
    }

    IEnumerator Roam()
    {
        while (true)
        {
            // Determine the point around which to roam
            Vector3 center = (roamCenter != null) 
                             ? roamCenter.position 
                             : transform.position;

            // Pick a random point in a sphere around that center
            Vector3 randomPoint = center + Random.insideUnitSphere * roamRadius;
            NavMeshHit hit;
            // Find nearest NavMesh location to that point
            if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);

                // Wait until the NPC arrives
                while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                    yield return null;

                // Pause before the next wander
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                // If sampling failed, retry next frame
                yield return null;
            }
        }
    }
}