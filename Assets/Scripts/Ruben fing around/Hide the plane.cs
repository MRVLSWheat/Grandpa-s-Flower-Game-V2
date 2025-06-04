using UnityEngine;
using System.Collections;

public class Hidetheplane : MonoBehaviour
{

    public void Start()
    {
        gameObject.SetActive(false);
    }
    public IEnumerator ActivatePlantAlert()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}