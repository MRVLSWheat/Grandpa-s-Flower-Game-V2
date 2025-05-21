using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalWarning : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("How close the player must be to trigger the warning.")]
    public float warningRange = 5f;

    [Header("Warning Effects")]
    [Tooltip("Prefab for the warning symbol (e.g., an exclamation mark). If left empty, a placeholder cube will be created.")]
    public GameObject warningSymbolPrefab;

    [Tooltip("Duration (in seconds) the symbol stays visible after leaving the range.")]
    public float symbolDuration = 2f;

    [Tooltip("Sound to play when the warning is triggered. If left empty, a placeholder beep will be generated.")]
    public AudioClip warningSound;

    private Transform playerTransform;
    private AudioSource audioSource;
    private GameObject currentSymbol;
    private bool isWarningActive = false;

    void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("No GameObject with tag 'Player' found in the scene.");

        // Set up AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Generate placeholder beep if no sound assigned
        if (warningSound == null)
        {
            warningSound = CreateBeepAudioClip();
        }
        audioSource.clip = warningSound;
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= warningRange)
        {
            if (!isWarningActive)
                TriggerWarning();
        }
        else
        {
            if (isWarningActive)
                ResetWarning();
        }
    }

    private void TriggerWarning()
    {
        isWarningActive = true;

        // Cancel any pending destroy
        CancelInvoke(nameof(DestroySymbol));

        // Instantiate warning symbol or create placeholder if none exists
        if (currentSymbol == null)
        {
            if (warningSymbolPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 2f;
                currentSymbol = Instantiate(warningSymbolPrefab, spawnPos, Quaternion.identity, transform);
            }
            else
            {
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.transform.SetParent(transform);
                placeholder.transform.localPosition = Vector3.up * 2f;
                placeholder.transform.localScale = Vector3.one * 0.5f;
                Renderer rend = placeholder.GetComponent<Renderer>();
                rend.material.color = Color.yellow;
                currentSymbol = placeholder;
            }
        }

        // Play warning sound
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
    }

    private void ResetWarning()
    {
        isWarningActive = false;
        // Schedule symbol destruction after duration
        Invoke(nameof(DestroySymbol), symbolDuration);
    }

    private void DestroySymbol()
    {
        if (currentSymbol != null)
        {
            Destroy(currentSymbol);
            currentSymbol = null;
        }
    }

    private AudioClip CreateBeepAudioClip()
    {
        int sampleRate = 44100;
        float durationSec = 0.5f;
        int sampleCount = Mathf.CeilToInt(sampleRate * durationSec);
        AudioClip clip = AudioClip.Create("Beep", sampleCount, 1, sampleRate, false);
        float frequency = 440f; // A4 note
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
        }
        clip.SetData(samples, 0);
        return clip;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, warningRange);
    }
}