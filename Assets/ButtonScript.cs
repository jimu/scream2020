using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonScript : MonoBehaviour
{

    public void StartLevel()
    {
        SceneManager.LoadScene("scene3");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
