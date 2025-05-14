using UnityEngine;

public class Animal : MonoBehaviour
{
    public float disturbanceIncreaseRate = 1f;  // Rate at which disturbance increases when near
    public float detectionRange = 5f;           // The range at which the animal detects the player
    private Transform player;                   // Reference to the player's transform

    void Start()
    {
        // Find the player object by tag (ensure your player has the "Player" tag)
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Calculate the distance between the player and the animal
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within detection range, increase disturbance
        if (distanceToPlayer <= detectionRange)
        {
            // Increase the disturbance value by a small amount over time
            DisturbanceManager.Instance.IncreaseDisturbance(disturbanceIncreaseRate * Time.deltaTime);
        }
    }
}
