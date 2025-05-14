using UnityEngine;

/// <summary>
/// Handles basic flower harvesting functionality
/// Destroys flower object on harvest
/// </summary>
public class Harvestable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Visual effect when harvested (optional)")]
    [SerializeField] private ParticleSystem harvestEffect;

    [Tooltip("Sound effect when harvested (optional)")]
    [SerializeField] private AudioClip harvestSound;

    /// <summary>
    /// Called when player harvests this flower
    /// </summary>
    public void Harvest()
    {
        // Play effects if assigned
        if (harvestEffect != null)
        {
            Instantiate(harvestEffect, transform.position, Quaternion.identity);
        }

        if (harvestSound != null)
        {
            AudioSource.PlayClipAtPoint(harvestSound, transform.position);
        }

        // Destroy the flower object
        Destroy(gameObject);

        Debug.Log("Flower harvested!");
    }
}