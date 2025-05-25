using UnityEngine;

public class testScript : MonoBehaviour
{
    public FirstQuestAudio firstQuestAudio;

    public void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            if (firstQuestAudio != null)
            {
                firstQuestAudio.StartFirstQuestAudio(); 
            }
            else
            {
                Debug.LogError("FirstQuestAudio reference is not set.");
            }
        }
    }
}
