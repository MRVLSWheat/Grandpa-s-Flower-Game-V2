using UnityEngine;

public class toactuvateplane : MonoBehaviour
{
    [SerializeField] private Hidetheplane windmillUI; // Drag the object in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("U key pressed here");
            StartCoroutine(windmillUI.ActivatePlantAlert());
        }
    }
}
