using Unity.VisualScripting;
using UnityEngine;

public class Colectable : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(gameObject.name);
            GameObject.Destroy(gameObject);
        }
    }
}
