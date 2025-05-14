using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HostileAnimalSimple : MonoBehaviour
{
    public float detectionRange = 5f;
    public float spawnRadius = 3f;
    public LayerMask groundLayer;

    private static bool isPlayerFlagged = false;

    private GameObject player;
    private GameObject respawnPoint;

    private Image fadePanel;
    private Canvas fadeCanvas;

    private bool hasTriggered = false;
    private Collider[] animalColliders;

    private Rigidbody playerRigidbody; // To temporarily disable physics
    private Collider playerCollider; // To temporarily disable collider
    private bool playerInputEnabled = true; // Track if the player can move

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        respawnPoint = GameObject.Find("RespawnPoint");

        if (player == null || respawnPoint == null)
        {
            Debug.LogError("Player or RespawnPoint not found. Make sure they exist and are named/tagged correctly.");
            enabled = false;
            return;
        }

        playerRigidbody = player.GetComponent<Rigidbody>();
        playerCollider = player.GetComponent<Collider>();

        // If the player does not have a Rigidbody, let us know
        if (playerRigidbody == null)
        {
            Debug.LogWarning("Player does not have a Rigidbody component. Teleportation may not work as expected.");
        }

        CreateFadeCanvas();
        animalColliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (hasTriggered || isPlayerFlagged) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= detectionRange)
        {
            hasTriggered = true;
            isPlayerFlagged = true;
            StartCoroutine(TeleportWithFade());
        }
    }

    IEnumerator TeleportWithFade()
    {
        // Disable animal colliders so they can't push the player
        foreach (var col in animalColliders)
            col.enabled = false;

        // Fade to black quickly
        yield return Fade(1f, 0.4f);

        // Debugging teleportation
        Debug.Log("Teleporting the player...");

        // Temporarily disable the Rigidbody's physics to prevent interference
        if (playerRigidbody != null)
            playerRigidbody.isKinematic = true;

        // Temporarily disable player collider to prevent collision interference
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Disable player input to prevent movement during teleportation
        playerInputEnabled = false;

        // Teleport
        Vector3 targetPos = GetRandomSpawnPosition();
        Debug.Log($"Teleporting player to position: {targetPos}");

        // Directly set player position (Use SetPositionAndRotation for better control)
        player.transform.SetPositionAndRotation(targetPos, player.transform.rotation);

        // If the player has a Rigidbody, make sure to turn kinematic mode back off
        if (playerRigidbody != null)
            playerRigidbody.isKinematic = false;

        // Re-enable player collider
        if (playerCollider != null)
            playerCollider.enabled = true;

        // Re-enable player input
        playerInputEnabled = true;

        // Fade back slowly
        yield return Fade(0f, 2f);

        // Re-enable animal colliders
        foreach (var col in animalColliders)
            col.enabled = true;

        // Reset flags
        isPlayerFlagged = false;
        hasTriggered = false;
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Try finding a valid position around the respawn point
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 testPos = respawnPoint.transform.position + randomOffset + Vector3.up * 10f;

            // Check if the position is on the ground
            if (Physics.Raycast(testPos, Vector3.down, out RaycastHit hit, 20f, groundLayer))
            {
                return hit.point + Vector3.up * 1f;
            }
        }
        // If no valid position is found, return respawnPoint position
        return respawnPoint.transform.position;
    }

    void CreateFadeCanvas()
    {
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        fadePanel = new GameObject("FadePanel").AddComponent<Image>();
        fadePanel.transform.SetParent(fadeCanvas.transform, false);

        RectTransform rt = fadePanel.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadePanel.color = new Color(0, 0, 0, 0);
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadePanel.color.a;
        float time = 0f;

        while (time < duration)
        {
            float a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadePanel.color = new Color(0, 0, 0, a);
            time += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
    }
}
