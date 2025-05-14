using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("Home Area")]
    public Transform homeCenter;
    public float roamRadius = 10f;
    public float roamDelay = 3f;

    [Header("Detection & Behavior")]
    public float detectionRadius = 15f;
    public float disturbanceThreshold = 50f;
    public float stopDistanceToPlayer = 2f;
    public float chaseCooldown = 5f;
    public float chaseSpeed = 3.5f;
    public float roamSpeed = 2f;

    [Header("Capture Settings")]
    public Transform respawnPoint;

    [Header("UI Fade Settings")]
    public Image fadeImage;              // Will be created at runtime if null
    public float fadeDuration = 1f;      // Time to fade out/in

    [Header("Wander Pause Settings")]
    public bool pauseAtWanderPoint = true;
    public float pauseDuration = 2f;

    private NavMeshAgent agent;
    private float roamTimer;
    private bool isChasingPlayer;
    private bool inCooldown;
    private float cooldownTimer;
    private bool isPausedAtWanderPoint = false;
    private float pauseTimer = 0f;

    private void Awake()
    {
        // Ensure we have a fade Image
        if (fadeImage == null)
            CreateFadeImage();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        roamTimer = roamDelay;
        NPCManager.Instance.RegisterNPC(this);
    }

    private void Update()
    {
        if (NPCManager.Instance.Player == null)
            return;

        float disturbance = DisturbanceManager.Instance.disturbanceValue;
        Transform player = NPCManager.Instance.Player;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (inCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                inCooldown = false;
            return;
        }

        if (isChasingPlayer)
        {
            if (distanceToPlayer <= stopDistanceToPlayer)
            {
                TeleportPlayer(player);
                ResetEngagementWithCooldown();
                return;
            }

            TryChasePlayer();
        }
        else
        {
            if (disturbance >= disturbanceThreshold && distanceToPlayer <= detectionRadius)
            {
                TryChasePlayer();
            }
            else
            {
                HandleWandering();
            }
        }
    }

    private void TryChasePlayer()
    {
        if (agent == null || NPCManager.Instance.Player == null)
            return;

        isChasingPlayer = true;
        agent.speed = chaseSpeed;

        Vector3 playerPos = NPCManager.Instance.Player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(playerPos, out hit, 10f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(playerPos);
    }

    private void TeleportPlayer(Transform player)
    {
        if (respawnPoint == null)
        {
            Debug.LogError("[NPCController] No respawnPoint set!");
            return;
        }

        StartCoroutine(FadeAndTeleport(player));
    }

    private IEnumerator FadeAndTeleport(Transform player)
    {
        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Teleport logic
        NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>();
        if (playerAgent != null)
        {
            playerAgent.Warp(respawnPoint.position);
        }
        else
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.position = respawnPoint.position;
                cc.enabled = true;
            }
            else
            {
                player.position = respawnPoint.position;
            }
        }

        Debug.Log($"Player captured by {name} and sent to respawn.");

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        fadeImage.color = new Color(c.r, c.g, c.b, endAlpha);
    }

    private void HandleWandering()
    {
        if (pauseAtWanderPoint && isPausedAtWanderPoint)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPausedAtWanderPoint = false;
                roamTimer = roamDelay;
            }
            return;
        }

        roamTimer += Time.deltaTime;
        if (roamTimer >= roamDelay && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Wander();
            roamTimer = 0f;
        }
    }

    private void Wander()
    {
        if (agent == null || homeCenter == null)
            return;

        isChasingPlayer = false;
        agent.speed = roamSpeed;

        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0f;
        Vector3 targetPosition = homeCenter.position + randomDirection;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(targetPosition, out navHit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
            if (pauseAtWanderPoint)
            {
                isPausedAtWanderPoint = true;
                pauseTimer = pauseDuration;
            }
        }
        else
        {
            Debug.LogWarning($"[NPC] Wander: No valid NavMesh position found near {targetPosition}.");
        }
    }

    public void ResetEngagement()
    {
        isChasingPlayer = false;
        agent.speed = roamSpeed;
        roamTimer = roamDelay;
    }

    private void ResetEngagementWithCooldown()
    {
        ResetEngagement();
        inCooldown = true;
        cooldownTimer = chaseCooldown;
    }

    /// <summary>
    /// Creates a fullscreen black Image for fade if one isn't assigned.
    /// </summary>
    private void CreateFadeImage()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("FadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Image
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        Image img = imageGO.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);

        // Stretch to fullscreen
        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadeImage = img;
    }
}
