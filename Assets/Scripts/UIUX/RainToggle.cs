using UnityEngine;

public class RainToggle : MonoBehaviour
{
    public GameObject Rain; // Assign your Rain GameObject in the Inspector

    void Start()
    {
        if (Rain != null)
        {
            Rain.SetActive(false);
        }
    }

    public void RainToggled(float value)
    {
        if (Rain != null)
        {
            if (value >= 60f && value <= 100f)
            {
                Rain.SetActive(true);
            }
            else
            {
                Rain.SetActive(false);
            }
        }
    }
}
