using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnPlayerOnProximity : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Distance at which the player is considered 'caught' and will respawn")]
    public float catchRadius = 1.5f;

    [Tooltip("How long to fade out/in (seconds)")]
    public float fadeDuration = 0.5f;

    private GameObject playerObj;
    private Vector3 respawnPosition;
    private Image fadeImage;
    private bool wasFar = true;
    private bool isFading = false;

    void Start()
    {
        // 1) Find the Player by tag
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError($"[{name}] No GameObject found with tag 'Player'.");
            enabled = false;
            return;
        }

        // 2) Remember its start position
        respawnPosition = playerObj.transform.position;

        // 3) Find or create a full-screen fade Image
        fadeImage = FindObjectOfType<Image>(true); 
        if (fadeImage == null || fadeImage.gameObject.name != "AutoFadeImage")
        {
            // create Canvas
            var canvasGO = new GameObject("AutoFadeCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // create full-screen Image
            var imgGO = new GameObject("AutoFadeImage");
            imgGO.transform.SetParent(canvasGO.transform, false);
            fadeImage = imgGO.AddComponent<Image>();
            var rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
        }
    }

    void Update()
    {
        if (isFading) return;

        float dist = Vector3.Distance(transform.position, playerObj.transform.position);
        if (dist <= catchRadius && wasFar)
        {
            StartCoroutine(TeleportWithFade());
            wasFar = false;
        }
        else if (dist > catchRadius)
        {
            wasFar = true;
        }
    }

    private IEnumerator TeleportWithFade()
    {
        isFading = true;

        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t / fadeDuration));
            t += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = Color.black;

        // Teleport & zero velocity
        playerObj.transform.position = respawnPosition;
        var rb = playerObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t / fadeDuration));
            t += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);

        isFading = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchRadius);
    }
}
