using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
public void onStartClick()
    {
        SceneManager.LoadScene("Ruben trying more stuff and hope i dont kill the game");
    }

public void onExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
