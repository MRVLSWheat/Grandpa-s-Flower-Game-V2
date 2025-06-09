using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DisturbanceManager : MonoBehaviour
{
    public static DisturbanceManager Instance;

    public float disturbanceValue = 0f;
    public float maxDisturbance = 100f;
    public float minDisturbance = 0f;
    public float decayRate = 0.1f;
    public SimpleSkyboxFade skyboxFadeScript;  // Drag the Skybox object (with the script on it) into this in the Inspector
    public RainToggle RainToggle; // Drag the RainToggle script here

    public Slider disturbanceSlider;

    [Header("Slider Position Settings")]
    [SerializeField] private Vector2 sliderAnchorMin = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 sliderAnchorMax = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 sliderPivot = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 sliderAnchoredPosition = new Vector2(300f, 0f); // Editable in Inspector

    public float PlantAmount = 0f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Apply editable slider positioning
        if (disturbanceSlider != null)
        {
            RectTransform rt = disturbanceSlider.GetComponent<RectTransform>();
            rt.anchorMin = sliderAnchorMin;
            rt.anchorMax = sliderAnchorMax;
            rt.pivot = sliderPivot;
            rt.anchoredPosition = sliderAnchoredPosition;
        }
    }

    void Update()
    {

        if (disturbanceValue > minDisturbance)
        {
            disturbanceValue -= decayRate * Time.deltaTime;
            disturbanceValue = Mathf.Clamp(disturbanceValue, minDisturbance, maxDisturbance);
        }

        if (disturbanceSlider != null)
        {
            disturbanceSlider.value = disturbanceValue;
        }
        /*  if (skyboxFadeScript != null)
          {
              skyboxFadeScript.SetVisibilityByValue(disturbanceValue);
           }   */

        if (RainToggle != null)
        {
            RainToggle.RainToggled(disturbanceValue);
        }
    }

    public void IncreaseDisturbance(float amount)
    {
        disturbanceValue += amount;
        disturbanceValue = Mathf.Clamp(disturbanceValue, minDisturbance, maxDisturbance);
    }
}
