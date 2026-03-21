using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Load Level_01
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}