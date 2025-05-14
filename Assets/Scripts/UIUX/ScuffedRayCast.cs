using TMPro;
using UnityEngine;

public class ScuffedRayCast : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;

    private void OnCollisionEnter (Collision collision)
    { 
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("NPC") || collision.gameObject.CompareTag("Flower"))
            {
                float distanceToHit = Vector3.Distance(collision.transform.position, transform.position);
                if (distanceToHit <= 1f) // Adjust the range as needed
                {
                    tooltipText.text = "Press E to interact";
                }
                tooltipText.gameObject.SetActive(true);
            }
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision ended with: " + collision.gameObject.name);
       // tooltipText.gameObject.SetActive(false);
    }
}
