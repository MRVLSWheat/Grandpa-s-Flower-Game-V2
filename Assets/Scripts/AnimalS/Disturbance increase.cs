using UnityEngine;

[RequireComponent(typeof(AnimalMovement))]
public class AnimalDisturbance : MonoBehaviour
{
    [Tooltip("How much disturbance to add each tick")]
    public float disturbanceAmount = 5f;

    private AnimalMovement mover;

    void Awake()
    {
        mover = GetComponent<AnimalMovement>();
        mover.OnStartFlee += HandleFlee;
        mover.OnFleeTick  += HandleFlee;
    }

    void OnDestroy()
    {
        mover.OnStartFlee -= HandleFlee;
        mover.OnFleeTick  -= HandleFlee;
    }

    private void HandleFlee()
    {
        if (DisturbanceManager.Instance != null)
            DisturbanceManager.Instance.IncreaseDisturbance(disturbanceAmount);
    }
}
