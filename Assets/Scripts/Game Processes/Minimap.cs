using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class AutoMinimap : MonoBehaviour
{
    [Header("Minimap Settings")]
    [Tooltip("Height of the minimap camera above the player.")]
    public float mapHeight = 50f;
    [Tooltip("Orthographic size of the minimap camera.")]
    public float mapSize = 50f;
    [Tooltip("Resolution (width and height) of the minimap RenderTexture.")]
    public int textureSize = 256;
    [Tooltip("Layer mask for objects to include in the minimap.")]
    public LayerMask minimapMask = ~0;

    [Header("UI Settings")]
    [Tooltip("Size of the minimap UI RawImage in pixels.")]
    public Vector2 minimapUISize = new Vector2(200, 200);
    [Tooltip("Screen-space position (bottom-left) of the minimap in pixels.")]
    public Vector2 minimapUIPosition = new Vector2(10, 10);
    [Tooltip("Sprite used for the player blip. If unset, uses default UI sprite.")]
    public Sprite playerIconSprite;

    private Camera mapCam;
    private RawImage mapImage;
    private RectTransform mapRect;
    private RectTransform iconRect;
    private Canvas canvas;

    void Awake()
    {
        SetupMinimap();
    }

    void LateUpdate()
    {
        FollowPlayer();
        RotateIcon();
    }

    void SetupMinimap()
    {
        // 1. Create and configure the minimap camera
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

        // 2. Create the RenderTexture
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 16);
        rt.name = "MinimapRT";
        mapCam.targetTexture = rt;

        // 3. Find or create a Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cGO = new GameObject("MinimapCanvas");
            canvas = cGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cGO.AddComponent<CanvasScaler>();
            cGO.AddComponent<GraphicRaycaster>();
        }

        // 4. Create the RawImage for the minimap
        GameObject rawGO = new GameObject("MinimapImage");
        rawGO.transform.SetParent(canvas.transform);
        mapImage = rawGO.AddComponent<RawImage>();
        mapImage.texture = rt;
        mapRect = rawGO.GetComponent<RectTransform>();
        mapRect.anchorMin = mapRect.anchorMax = new Vector2(0, 0);
        mapRect.pivot = new Vector2(0, 0);
        mapRect.anchoredPosition = minimapUIPosition;
        mapRect.sizeDelta = minimapUISize;

        // 5. Create the player blip icon
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

    void FollowPlayer()
    {
        if (mapCam != null)
        {
            Vector3 pos = transform.position;
            mapCam.transform.position = new Vector3(pos.x, pos.y + mapHeight, pos.z);
        }
    }

    void RotateIcon()
    {
        if (iconRect != null)
        {
            float angle = transform.eulerAngles.y;
            iconRect.localEulerAngles = new Vector3(0, 0, -angle);
        }
    }
}
