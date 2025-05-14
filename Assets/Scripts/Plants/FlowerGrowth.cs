using System.Collections;
using UnityEngine;

public class FlowerGrowth : MonoBehaviour
{
    public float growthTime = 6f;
    public float maxHeight = 2f;
    public bool canBeDestroyed = false; // New: Tracks if cube is ready to be destroyed

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        float timer = 0f;

        while (timer <= growthTime)
        {
            float progress = timer / growthTime;
            float newHeight = Mathf.Lerp(startScale.y, maxHeight, progress);
            transform.localScale = new Vector3(startScale.x, newHeight, startScale.z);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(startScale.x, maxHeight, startScale.z);
        canBeDestroyed = true; // Now the cube can be destroyed
    }
}