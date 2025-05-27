using UnityEngine;
using UnityEngine.UI;

public class DisturbanceMeter : MonoBehaviour
{
    // Color values
    [Header("Color Zones")]
    public Color green = Color.green;
    public Color yellow = Color.yellow;
    public Color orange = new Color(1, 0.5f, 0);
    public Color red = Color.red;
    [SerializeField] private float colorChangeSpeed = 5f;

    // The Pusle effect when player reaches 70> in Disturbance
    [Header("Danger Effect")]
    public float pulseSpeed = 2f;           // Speed of the pulse effect
    public float pulseIntensity = 0.5f;     // How strong the fade pulses

    private Slider slider;
    private Image fillImage;
    private Color targetColor;

    void Awake()
    {
        slider = GetComponent<Slider>();
        fillImage = slider.fillRect.GetComponent<Image>();
        targetColor = green;
    }

    void Update()
    {
        // Set target color based on disturbance level
        float value = slider.value;
        targetColor = value <= 25 ? green :
                     value <= 50 ? yellow :
                     value <= 70 ? orange : 
                     red;

        // Apply pulse effect ONLY in red zone (70+)
        if (value >= 70)
        {
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensity);
            fillImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f - alpha);
        }
        else
        {
            // Smooth transition for non-red zones
            fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * colorChangeSpeed);
        }
    }
}