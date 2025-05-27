using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class AutoMinimap : MonoBehaviour
{
    [Header("Minimap Settings")]
    public float mapHeight = 50f;
    public float mapSize = 50f;
    public int textureSize = 256;
    public LayerMask minimapMask = ~0;
    
    [Header("UI Settings")]
    public Vector2 minimapUISize = new Vector2(200, 200);
    public Vector2 minimapUIPosition = new Vector2(10, 10);
    public Sprite playerIconSprite;
    
    [Header("Toggle Settings")]
    [Tooltip("Key to toggle the minimap on/off.")]
    public KeyCode toggleKey = KeyCode.M;

    private Camera mapCam;
    private RawImage mapImage;
    private RectTransform mapRect;
    private RectTransform iconRect;
    private Canvas canvas;
    private bool minimapEnabled = true;

    void Awake()
    {
        SetupMinimap();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMinimap();
        }
    }

    void LateUpdate()
    {
        if (!minimapEnabled) return;
        FollowPlayer();
        RotateIcon();
    }

    void SetupMinimap()
    {
        // Create minimap camera
        GameObject camGO = new GameObject("MinimapCamera");
        camGO.transform.SetParent(transform);
        camGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        camGO.transform.localPosition = new Vector3(0, mapHeight, 0);
        mapCam = camGO.AddComponent<Camera>();
        mapCam.orthographic = true;
        mapCam.orthographicSize = mapSize;
        mapCam.cullingMask = minimapMask;
        mapCam.clearFlags = CameraClearFlags.SolidColor;
        mapCam.backgroundColor = Color.clear;

        // Create render texture
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 16);
        rt.name = "MinimapRT";
        mapCam.targetTexture = rt;

        // Find or create canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cGO = new GameObject("MinimapCanvas");
            canvas = cGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cGO.AddComponent<CanvasScaler>();
            cGO.AddComponent<GraphicRaycaster>();
        }

        // Create minimap UI
        GameObject rawGO = new GameObject("MinimapImage");
        rawGO.transform.SetParent(canvas.transform);
        mapImage = rawGO.AddComponent<RawImage>();
        mapImage.texture = rt;
        mapRect = rawGO.GetComponent<RectTransform>();
        mapRect.anchorMin = mapRect.anchorMax = new Vector2(0, 0);
        mapRect.pivot = new Vector2(0, 0);
        mapRect.anchoredPosition = minimapUIPosition;
        mapRect.sizeDelta = minimapUISize;

        // Create player blip icon
        GameObject iconGO = new GameObject("MinimapIcon");
        iconGO.transform.SetParent(rawGO.transform);
        Image icon = iconGO.AddComponent<Image>();
        icon.sprite = playerIconSprite != null
            ? playerIconSprite
            : Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(16, 16);
    }

    void ToggleMinimap()
    {
        minimapEnabled = !minimapEnabled;
        // Toggle camera and UI
        if (mapCam != null) mapCam.enabled = minimapEnabled;
        if (mapImage != null) mapImage.gameObject.SetActive(minimapEnabled);
        if (iconRect != null) iconRect.gameObject.SetActive(minimapEnabled);
    }

    void FollowPlayer()
    {
        Vector3 pos = transform.position;
        mapCam.transform.position = new Vector3(pos.x, pos.y + mapHeight, pos.z);
    }

    void RotateIcon()
    {
        float angle = transform.eulerAngles.y;
        iconRect.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
