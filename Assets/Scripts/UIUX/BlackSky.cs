using UnityEngine;

public class SimpleSkyboxFade : MonoBehaviour
{
    //private Renderer rend;
    //private Color baseColor;
    public GameObject Rain;

    void Start()
    {
     //   rend = GetComponent<Renderer>();
      //  baseColor = rend.material.color;

        // Set initial alpha to 0 (invisible)
      //  Color color = baseColor;
      //  color.a = 0f;
      //  rend.material.color = color;
        Rain.SetActive(false); // Ensure rain is initially inactive
    }

    // Call this method from another script
    public void SetVisibilityByValue(float value)
    {
       // float alpha = 0f;

        if (value >= 90f && value <= 100f)
        {
            // Map 90–100 to 0–1 alpha if needed
            // alpha = (value - 90f) / 10f;
            Rain.SetActive(true);
        }
        else
        {
            Rain.SetActive(false);
        }

       // Color color = baseColor;
        //color.a = alpha;
       // rend.material.color = color;
    }
}