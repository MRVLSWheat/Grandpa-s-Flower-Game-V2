using System.Collections;
using UnityEngine;

public class ChangingtoGame : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ChangeToGame());
    }

    public IEnumerator ChangeToGame()
    {
        yield return new WaitForSeconds(13f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("FinalGame");
    }
}
